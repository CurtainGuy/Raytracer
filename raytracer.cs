using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace Template
{
    // Raytracer beheert de camera, de scene en tekent alle rays. 
    class Raytracer
    {
        // Member variabelen
        public Surface screen;
        
        public Camera camera;
        public Scene scene;

        // In het debugscherm zijn deze coördinaten het middenpunt. Hier staat de camera op. 
        // Zijn veel gehardcode. Niet aangeraden om te veranderen...

        public int CameraX = 767;
        public int CameraZ = 500;

        Surface pattern;
        Surface sky;
        
        // Variabelen die aangepast kunnen worden.:
        //--------------------------------------------------------------
        // Field of View in graden. 
        public float fov = 90;


        // De coördinaten van het punt waar de camera naar kijkt. 
        public float xrichting = 0;
        public float yrichting = 0;
        public float zrichting = 1;
        // Het aantal keer dat een secondary ray gemaakt mag worden.
        int maxRecursion = 10;

        
        //---------------------------------------------------------------


        public void Init()
        {
            Vector3 camerarichting = new Vector3(xrichting, yrichting, zrichting);
            camerarichting.Normalize();

            // De camera wordt aangemaakt. Startpositie is 0,0,0. Richting is naar de Z-richting. 
            camera = new Camera(new Vector3(0, 0, 0), camerarichting, fov);
            
            scene = new Scene(camera.CameraPosition);
            // Het scherm bestaat uit twee 512 bij 512 dingen.
            screen = new Surface(1024, 512);
            
            // De patronen.
            pattern = new Surface("../../assets/pattern.png");
            sky = new Surface("../../assets/stpeters_probe.png");

            // Render wordt aangeroepen bij het aanmaken.
            Render();
        }
        
        public void Tick()
        {
            DrawDebug();
            
            // Camera coördinaten worden afgerond op 3 decimalen en wordt geprint. 
            screen.Print("Cameraposition:" + new Vector3((float)Math.Round(camera.CameraPosition.X, 3), 
                (float)Math.Round(camera.CameraPosition.Y, 3), 
                (float)Math.Round(camera.CameraPosition.Z, 3)), 
                512, 490, 0xffffff);
        }

        
        public void Render()
        {
            // Een tijdelijk array wordt gemaakt om de kleuren van het scherm in te stoppen.
            // Dit zorgt ervoor dat de debug niet over het scherm wordt geprint.
            Vector3[] colors = new Vector3[512 * 512];

            // De i houdt bij welke positie van de array er wordt gebruikt. 
            int i = 0;
            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    // De camera heeft de richtingen per pixel opgeslagen in een array, en maakt hier nu een ray van. 
                    Ray ray = camera.SendRay(i);

                    // Elke zoveel rays op y = 256 worden getekent.
                    if (y == 256 && x % 20 == 0)
                    {
                        // De uiteindelijke kleur van de ray wordt hier opgeslagen.
                        colors[i] = Trace(ray, true, maxRecursion);
                        // De primary rays beginnen met een rood cirkelsegment van lengte 1, zoals in de opdracht vermeld wordt.
                        DrawDebugRay(new Ray(ray.O, ray.D, 1), new Vector3(255,0,0));
                    }
                    else
                        // Als het nit in de debug getekent moet worden wordt er false meegegeven.
                        colors[i] = Trace(ray, false, maxRecursion);
                    i++;
                }
                
            }
            // Vervolgens worden de camera, screen en primitives (alleen spheres) getekent. 
            DrawDebug();

            // Tenslotte wordt de scene echt geplot.
            i = 0;
            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    screen.Plot(x, y, FixColor(colors[i]));
                    i++;
                }
            }
        }

        // De trace functie van de slides.
        Vector3 Trace(Ray ray, bool debug, int recursion)
        {
            // Er wordt naar een Intersection gezocht
            Intersection I = SearchIntersect(ray);

            // Als er geen intersectie is gevonden...
            if (I.p == null)
            {
                // Tekent hij voor debugrays ook de rays in de debugwindow.
                if (debug)
                {
                    // Primary rays zijn anders getekent.
                    if (recursion == maxRecursion)
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, ray.t - 1), new Vector3(255, 255, 0));
                    else
                        DrawDebugRay(ray, new Vector3(0, 255, 0));

                }
                // Als er niets wordt gevonden, wordt de skydome getekent.
                return CreateSkyDome(ray);
            }
            Vector3 color = I.p.color;
            // Als het geintersecte object een mirror is....
            if (I.p.Mirror)
            {
                // Tekent hij voor debugrays ook de rays in de debugwindow.
                if (debug)
                {
                    if (recursion == maxRecursion)
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, I.d - 1), new Vector3(255, 255, 0));
                    else
                        DrawDebugRay(new Ray(ray.O, ray.D, I.d), new Vector3(0, 255, 0));
                }

                // Als recursie kleiner dan 0 is, dan wordt er zwart getekent.
                if (recursion < 0)
                {
                    return Vector3.Zero;
                }

                // Methode om een ray te reflecteren.
                // de kleur van de gereflecteerde ray vermenigvuldigd met hoe sterk de reflectie is
                // de kleur van het object wordt vermenigvuldigd met hoe sterk de absorptie is
                return ((Trace(Reflect(ray, I), debug, recursion - 1) * color * I.p.reflection) + (DirectIllumination(I, debug) * I.p.absorption));
            }
            else
            {
                if (I.p is Plane)
                    color = CreatePattern(I.i);
                if (debug)
                {
                    if (recursion == maxRecursion)
                        DrawDebugRay(new Ray(ray.D + ray.O, ray.D, I.d - 1), new Vector3(255, 255, 0));
                    else
                        DrawDebugRay(new Ray(ray.O, ray.D, I.d), new Vector3(0, 255, 0));
                }
                // PLanes hebben een patroon nodig.
                if (I.p is Plane)
                    return (DirectIllumination(I, debug) * CreatePattern(I.i));
                
                return (DirectIllumination(I, debug) * I.p.color);
            }
        }

        // Methode om een ray te reflecten. Geheel volgens de slides.
        public Ray Reflect(Ray ray, Intersection I)
        {
            Ray seccondaryRay = new Ray();
            seccondaryRay.D = ray.D - ((2 * I.n) * (Vector3.Dot(ray.D, I.n)));
            seccondaryRay.O = I.i;
            seccondaryRay.t = int.MaxValue;
            return seccondaryRay;
        }


        // Methode om refracted ray te berekenen. Nog niet geimplementeerd. 
        public Ray Refract(Ray ray, Intersection I, float N1, float N2)
        {
            float angle1 = Vector3.Dot(I.n, ray.D) / (I.n.Length * ray.D.Length);

            //N = 1;       brekingsindex van lucht
            //N= (3 / 2); brekingsindex van glass
            //N = (4 / 3); brekingsindex van water

            float angle2 = (float)Math.Asin((N1 / N2) * Math.Sin(angle1));

            Vector3 refractive = -I.n * angle2;
            return new Ray(I.i, refractive, ray.t);
        }
        
        // Berekent de belichting na een intersectie. 
        Vector3 DirectIllumination(Intersection I, bool debug)
        {
            Vector3 illumination = new Vector3(0, 0, 0);
            // Maakt een shadowray 
            Ray shadowRay = new Ray();
            foreach (LightSource light in scene.lightsources)
            {
                // Veranderd de shadowray voor elke lightsource.
                shadowRay.D = light.Position - I.i;
                shadowRay.O = I.i;
                shadowRay.t = shadowRay.D.Length;
                shadowRay.D.Normalize();

                // Shadowray wordt getekent bij debug.
                if(debug)
                    DrawDebugRay(new Ray(I.i, shadowRay.D, shadowRay.t), new Vector3(200, 200, 200));

                // Checkt of de lightsource visible is, zo niet, return zwart
                if (!IsVisible(I, shadowRay)) continue; // Zwart.

                // Berekent de kleur naar de slides' methode.
                float attenuation = light.Intensity / (shadowRay.t * shadowRay.t);
                float NdotL = Vector3.Dot(I.n, shadowRay.D);
                if (NdotL < 0) continue;
                // Dotproduct van de normaal van de intersectie en de shadowray.
                illumination = light.Color * attenuation * NdotL;
                continue;
            }

            return illumination; 
        }
        
        // Kijkt of de lijn tussen het lichtpuntje en een punt niet wordt verhinderd.
        public bool IsVisible(Intersection I, Ray shadowRay)
        {
            float tMin = int.MaxValue;
            foreach (Primitive p in scene.primitives)
            {
                // De dichtbijzijnde intersectie wordt genomen.
                float t = p.Intersect(shadowRay).d;
                if (t > 0 && t < tMin)
                    tMin = t;
            }

            // als de kortste afstand gelijk is aan de lengte van de shadowray, 
            //dan zitten er geen objecten tussen de lightsource en shadowray
            if (tMin >= shadowRay.t)
                return true;
            else
                return false;
        }

        // Vind de primitive waarmee de ray intersect. Als er niets wordt gevonden returnt het een lege intersection. 
        Intersection SearchIntersect(Ray ray)
        {
            float tMin = int.MaxValue; //begin bij de grootste mogelijke hoeveelheid
            Primitive intersected = null; // als je nergens mee intersect, dan geef je niks terug
            foreach (Primitive p in scene.primitives)
            {
                float t = p.Intersect(ray).d;
                if (t > 0 && t < tMin)
                {
                    tMin = t; // de afstand tussen lightsource en intersection
                    intersected = p; // het gevonden object geef je door
                }
            }
            if (intersected == null)
                return new Intersection();
            // return de intersectie die gevonden is
            return new Intersection(intersected, tMin, intersected.Intersect(ray).n, intersected.Intersect(ray).i);
        }

        // Tekent een ray op de debug met de line segmenten.
        public void DrawDebugRay(Ray ray, Vector3 color )
        {
            screen.Line(ConverttoDebugX(ray.O.X),
                ConverttoDebugY(ray.O.Z),
                ConverttoDebugX((ray.D.X * ray.t) + ray.O.X),
                ConverttoDebugY((ray.D.Z * ray.t) + ray.O.Z), FixColor(color));
        }

        // Tekent de primitives, camera en screen.
        void DrawDebug()
        {            
            // Maakt de camera & screen aan in de debugwindow. 
            screen.Plot(CameraX, CameraZ, FixColor(new Vector3(255,255,255)));
            screen.Line(CameraX + (int)(camera.screenCorner0.X)*48, CameraZ - 48 - (int)(camera.screenCorner0.Z)*48,
                CameraX + (int)(camera.screenCorner1.X)*48, CameraZ - 48 - (int)(camera.screenCorner1.Z)*48, FixColor(new Vector3(255, 255, 255)));
            
            // Eerst worden de spheres gefilterd uit de lijst van primitieven.
            List<Sphere> spheres = new List<Sphere>();
            foreach(Primitive p in scene.primitives)
            {
                if (p.type == Type.Sphere)
                    spheres.Add((Sphere)p);
            }
            // Een kleine hoek wordt vantevoren berekent.
            float angle = 2 * (float)Math.PI / 100;

            // Voor elke sphere wordt het getekent in de debug window op y = cameraposition.Y
            foreach (Sphere s in spheres)
            {
                // Normale spheres worden getekent met hun eigen kleur. Mirrors worden altijd lichtblauw getekent.
                Vector3 c = s.color;
                if (s.Mirror)
                    c = new Vector3(240,255,255);
                
                // Omdat de grootte van de cirkel veranderd met de camerapositie.Y, moet er een nieuwe radius berekent worden.
                float newradius = (float)Math.Sqrt((s.Radius * s.Radius) - ((s.Origin.Y - camera.cameraPosition.Y) * (s.Origin.Y - camera.cameraPosition.Y)));
                if(newradius > 0)
                {
                    for (int a = 0; a < 100; a++)
                    {
                        // Tekent voor elke deel van de cirkel. 100 segmenten. Dure operatie...
                        screen.Line(ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos(a * angle))),
                            ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin(a * angle))),
                            ConverttoDebugX((float)(s.origin.X + newradius * Math.Cos((a + 1) * angle))),
                            ConverttoDebugY((float)(s.origin.Z + newradius * Math.Sin((a + 1) * angle))), FixColor(c));
                    }
                }
            }
        }
        // Zetten de world-coördinaten om in schermcoördinaten. Gebruikt 767 en 500 
        int ConverttoDebugX(float x)
        {
            return (int)(767 + (x * 48));
        }

        int ConverttoDebugY(float z)
        {
            return (int)(500 - (z * 48));
        }

        // Veranderd de kleur van Vectoren naar ints.
        public int FixColor(Vector3 color)
        {
            if (color.X > 255) color.X = 255;
            if (color.Y > 255) color.Y = 255;
            if (color.Z > 255) color.Z = 255;
            return ((int)color.X << 16) + ((int)color.Y << 8) + (int)(color.Z);
        }

        // Maakt het patroon.
        public Vector3 CreatePattern(Vector3 P)
        {
            Vector2 point = new Vector2(P.X, P.Z);
            int x = (int)Math.Round(point.X * pattern.width - 0.5);

            // zorg er voor dat x binnen het bereik van de afbeelding blijft, aangezien de plane oneindig lang is

            while (x < 0)
            {
                x += pattern.width;
            }
            while (x >= pattern.width)
            {
                x -= pattern.width;
            }

            int y = (int)Math.Round(point.Y * pattern.height - 0.5);

            // zorg er voor dat y binnen het bereik van de afbeelding blijft, aangezien de plane oneindig lang is

            while (y < 0)
            {
                y += pattern.height;
            }
            while (y >= pattern.height)
            {
                y -= pattern.height;
            }

            // geef de kleur van de afbeelding op het berekende punt terug
            Color col = pattern.bmp.GetPixel(x, y);
            return new Vector3(col.R, col.G, col.B);
        }

        // Kijkt waar een ray intersect met de skydome. Return de kleur.
        public Vector3 CreateSkyDome(Ray ray)
        {
            //formule van de gegeven site
            float r = (float)((1 / Math.PI) * Math.Acos(ray.D.Z) / Math.Sqrt(ray.D.X * ray.D.X + ray.D.Y * ray.D.Y + 0.01f));
            // schaal x naar de grootte van de afbeelding
            float x = MathHelper.Clamp(((ray.D.X * r + 1) * sky.width / 2), 0, sky.width - 1);
            // schaal y naar de grootte van de afbeelding
            float y = MathHelper.Clamp(((ray.D.Y * r + 1) * sky.height / 2), 0, sky.height - 1);
            // geef de kleur van de afbeelding op het berekende punt terug
            Color col = sky.bmp.GetPixel((int)x, (int)y);
            return new Vector3(col.R, col.G, col.B);
        }
    }
}