using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTalDrawSystem.SistemaAudio;
using UTalDrawSystem.SistemaDibujado;
using UTalDrawSystem.SistemaFisico;
using UTalDrawSystem.SistemaGameObject;

namespace UTalDrawSystem.MyGame
{
    public class Juego : Escena
    { 
        public Camara camara { private set; get; }

        Texture2D fondo;

        SpriteFont timer;
        SpriteFont vidasActuales;
        SpriteFont puntajeTotal;
        SpriteFont powerUpLevel;
        Texture2D timerIcon;
        Texture2D vidasIcon;
        Texture2D powerUpIcon;
        Texture2D asteroidIcon;
        Texture2D fondoPausa;


        int difficultyLevel;
        double timeChangeDifficulty;

        int cameraSpeed;

        public int score;

        public Nave ship;
        List<Background> listaFondo;
        List<PlanetaCuadrado> listaPlanetas;
        List<Asteroide> listaAsteroides;
        List<Agujero> listaAgujeros;
        List<Coleccionable> listaEnergy;

        List<Proyectil> listaProyectiles;

        Random rnd;
        public int n_Choques { private set; get; }
        bool collision_on;
        bool ganoVidas;
        public double time { private set; get; }
        double timeSpawnPlanet;
        double planetSpawnRange;
        double timeSpawnAsteroides;
        double timeSpawnAgujeros;
        double timeSpawnEnergy;
        int posYEnergy;

        public bool paused;
        bool isPauseKeyPressed;

        public Juego(ContentManager content)
        {
            UTGameObjectsManager.Init();

            fondo = content.Load<Texture2D>("fondo");

            timer = content.Load<SpriteFont>("Titulo");
            vidasActuales = content.Load<SpriteFont>("Titulo");
            puntajeTotal = content.Load<SpriteFont>("Instrucciones");
            powerUpLevel = content.Load<SpriteFont>("Instrucciones");

            timerIcon = content.Load<Texture2D>("timer");
            vidasIcon = content.Load<Texture2D>("vida");
            powerUpIcon = content.Load<Texture2D>("energy");
            asteroidIcon = content.Load<Texture2D>("Asteroid");

            fondoPausa = content.Load<Texture2D>("img");

            difficultyLevel = 1;
            cameraSpeed = 6;

            score = 0;

            listaFondo = new List<Background>();
            listaPlanetas = new List<PlanetaCuadrado>();
            listaAsteroides = new List<Asteroide>();
            listaAgujeros = new List<Agujero>();
            listaEnergy = new List<Coleccionable>();

            listaProyectiles = new List<Proyectil>();

            ship = new Nave(content, "ship", new Vector2(450, Game1.INSTANCE.GraphicsDevice.Viewport.Height), 0.3f, UTGameObject.FF_form.Rectangulo);

            rnd = new Random();
            time = 0;
            timeChangeDifficulty = 0;
            timeSpawnPlanet = 0;
            planetSpawnRange = rnd.Next(60,80);
            timeSpawnAsteroides = 0;
            timeSpawnAgujeros = 0;
            timeSpawnEnergy = 0;

            n_Choques = 0;
            ganoVidas = false;

            camara = new Camara(new Vector2(0,0), .5f, 0);
            camara.HacerActiva();

            listaFondo.Add(new Background("fondo", new Vector2(camara.pos.X, camara.pos.Y + Game1.INSTANCE.GraphicsDevice.Viewport.Height), 2, UTGameObject.FF_form.Rectangulo, true, false, true));
            listaFondo.Add(new Background("fondo", new Vector2(camara.pos.X + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, camara.pos.Y + Game1.INSTANCE.GraphicsDevice.Viewport.Height), 2, UTGameObject.FF_form.Rectangulo, true, false, true));

            paused = false;
            isPauseKeyPressed = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                if (!paused && !isPauseKeyPressed)
                {
                    isPauseKeyPressed = true;
                    paused = true;
                }
                else if (paused && !isPauseKeyPressed)
                {
                    isPauseKeyPressed = true;
                    paused = false;
                }
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.P))
            {
                isPauseKeyPressed = false;
            }

            if (!paused)
            {
                time += gameTime.ElapsedGameTime.TotalSeconds;
                timeChangeDifficulty += gameTime.ElapsedGameTime.TotalSeconds;
                timeSpawnPlanet += gameTime.ElapsedGameTime.TotalSeconds;
                timeSpawnAsteroides += gameTime.ElapsedGameTime.TotalSeconds;
                timeSpawnAgujeros += gameTime.ElapsedGameTime.TotalSeconds;
                timeSpawnEnergy += gameTime.ElapsedGameTime.TotalSeconds;

                camara.pos.X += cameraSpeed;

            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    Game1.INSTANCE.ChangeScene(Game1.Scene.Game);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.M))
                {
                    Game1.INSTANCE.ChangeScene(Game1.Scene.Start);
                }
            }


            Console.WriteLine(paused);

            if (timeChangeDifficulty  >= 15)
            {
                timeChangeDifficulty = 0;
                difficultyLevel++;
                
            }



            if (score >= ship.nuevaVida)
            {
                ship.nuevaVida += 5000;
                ship.vidas += 1;
            }

            if (ship.isShooting && ship.shootCD <= 0)
            {
                ship.isShooting = false;

                switch (ship.buffLevel)
                {
                    case 5:
                        ship.shootCD = 0.07f;
                        break;
                    default:
                        ship.shootCD = 0.1f;
                        break;
                }

                AudioManager.Play(AudioManager.Sounds.Disparo_1, true);

                switch (ship.buffLevel)
                {
                    case 1:                    
                        listaProyectiles.Add(new Proyectil("rayo", ship.objetoFisico.pos, 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        break;

                    case 2:
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y - 10), 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y + 10), 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        break;

                    case 3:
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y - 10), 50, -25, -0.3925f, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", ship.objetoFisico.pos, 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y + 10), 50, 25, 0.3925f, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        break;
                    case 4:
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y - 10), 50, -25, -0.3925f, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y - 10), 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y + 10), 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y + 10), 50, 25, 0.3925f, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        break;
                    case 5:
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y - 10), 50, -25, -0.3925f, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y - 10), 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y + 10), 50, 0, 0, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        listaProyectiles.Add(new Proyectil("rayo", new Vector2(ship.objetoFisico.pos.X, ship.objetoFisico.pos.Y + 10), 50, 25, 0.3925f, 1, UTGameObject.FF_form.Rectangulo, false, false));
                        break;
                }
            }

            if (listaProyectiles.Count > 0)
            {

                if (listaProyectiles.First<Proyectil>().isDestroyed)
                {
                    listaProyectiles.Remove(listaProyectiles.First<Proyectil>());
                }
                else if (listaProyectiles.First<Proyectil>().objetoFisico.pos.X > camara.pos.X + 200 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2)
                {
                    listaProyectiles.First<Proyectil>().Destroy();
                    listaProyectiles.Remove(listaProyectiles.First<Proyectil>());
                }

            }

            if (ship.objetoFisico.isColliding && !collision_on)
            {
                collision_on = true;
                n_Choques++;
            }
            else if (!ship.objetoFisico.isColliding)
            {
                collision_on = false;
            }

            /*SPAWNERS*********************************************************************************************************************************************************/

            if (listaFondo.Count > 0)
            {
                if (listaFondo.Last<Background>().objetoFisico.pos.X < camara.pos.X +  800)
                {
                    listaFondo.Add(new Background("fondo", new Vector2(camara.pos.X + 795 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, camara.pos.Y + Game1.INSTANCE.GraphicsDevice.Viewport.Height), 2, UTGameObject.FF_form.Rectangulo, true, false, true));
                    listaFondo.Last<Background>().objetoFisico.isTrigger = true;
                }

                if (listaFondo.First<Background>().objetoFisico.pos.X < camara.pos.X - 800)
                {
                    listaFondo.First<Background>().Destroy();
                    listaFondo.Remove(listaFondo.First<Background>());
                }
            }

            if (timeSpawnPlanet > planetSpawnRange)
            {
                listaPlanetas.Add(new PlanetaCuadrado("planetacuadrado", new Vector2(camara.pos.X + 600 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, rnd.Next((int)camara.pos.Y, (int)camara.pos.Y + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2)), 1.4f, UTGameObject.FF_form.Rectangulo, true, false, false));
                timeSpawnPlanet = 0;
                planetSpawnRange = rnd.Next(60,80);
            }
            if (listaPlanetas.Count > 0)
            {
                if (listaPlanetas.First<PlanetaCuadrado>().objetoFisico.pos.X < camara.pos.X - 600)
                {
                    listaPlanetas.First<PlanetaCuadrado>().Destroy();
                    listaPlanetas.Remove(listaPlanetas.First<PlanetaCuadrado>());
                }
            }

            if (timeSpawnAgujeros > 4f)
            {
                listaAgujeros.Add(new Agujero("black_hole", new Vector2(camara.pos.X + 600 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, rnd.Next((int)camara.pos.Y + 100, (int)camara.pos.Y - 100 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2)), 1, UTGameObject.FF_form.Circulo, false));
                timeSpawnAgujeros = 0;
            }
            if (listaAgujeros.Count > 0)
            {
                if (listaAgujeros.First<Agujero>().objetoFisico.pos.X < camara.pos.X - 200)
                {
                    listaAgujeros.First<Agujero>().Destroy();
                    listaAgujeros.Remove(listaAgujeros.First<Agujero>());
                }
            }

            if (timeSpawnAsteroides > 1.2f / difficultyLevel)
            {
                listaAsteroides.Add(new Asteroide("Asteroid", new Vector2(camara.pos.X + 600 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, rnd.Next((int)camara.pos.Y + 100, (int)camara.pos.Y - 100 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2)), 0.3f, UTGameObject.FF_form.Circulo, false, true));
                timeSpawnAsteroides = 0;
            }
            if (listaAsteroides.Count > 0)
            {
                if (listaAsteroides.First<Asteroide>().objetoFisico.pos.X < camara.pos.X - 200)
                {
                    listaAsteroides.First<Asteroide>().Destroy();
                    listaAsteroides.Remove(listaAsteroides.First<Asteroide>());
                }
            }

            if (timeSpawnEnergy > 5f)
            {
                posYEnergy = rnd.Next((int)camara.pos.Y + 100, (int)camara.pos.Y - 100 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2);
                listaEnergy.Add(new Coleccionable("energy", new Vector2(camara.pos.X + 600 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, posYEnergy), 0.1f, UTGameObject.FF_form.Circulo, false));
                timeSpawnEnergy = 0;
            }        
            if (listaEnergy.Count > 0)
            {
                if (listaEnergy.First<Coleccionable>().objetoFisico.pos.X < camara.pos.X - 200)
                {
                    listaEnergy.First<Coleccionable>().Destroy();
                    listaEnergy.Remove(listaEnergy.First<Coleccionable>());
                }
            }
            /*SPAWNERS*********************************************************************************************************************************************************/

            //Verifica si la nave es alcanzada por la cámara.
            if (ship.objetoFisico.pos.X < camara.pos.X)
            {
                if (!ship.invulnerable)
                {
                    ship.invulnerable = true;
                    ship.objetoFisico.pos = ship.Respawn();
                }
            }
            //Evita que salga por la parte izquierda cuando la nave es invulnerable.
            if (ship.invulnerable && ship.objetoFisico.pos.X <= camara.pos.X + 30)
            {
                ship.objetoFisico.pos = new Vector2(camara.pos.X + 30, ship.objetoFisico.pos.Y);
            }
            
            //Evita que la nave salga por la parte derecha
            if (ship.objetoFisico.pos.X >= camara.pos.X - 30 + Game1.INSTANCE.GraphicsDevice.Viewport.Width*2)
            {
                ship.objetoFisico.pos = new Vector2(camara.pos.X - 30 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2, ship.objetoFisico.pos.Y);
            }

            //Evita que la nave salga por la parte superior de la pantalla.
            if (ship.objetoFisico.pos.Y <= camara.pos.Y + 75)
            {
                ship.objetoFisico.pos = new Vector2(ship.objetoFisico.pos.X, camara.pos.Y + 75 );
            }
            
            //Evita que la nave salga por la parte inferior de la pantalla.
            if (ship.objetoFisico.pos.Y >= camara.pos.Y - 75 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2)
            {
                ship.objetoFisico.pos = new Vector2(ship.objetoFisico.pos.X, camara.pos.Y - 75 + Game1.INSTANCE.GraphicsDevice.Viewport.Height * 2);
            }

            //Gana vidas cada 25 monedas recogidas (supuestamente)
            if (ship.powerUpTotales > 0 && ship.powerUpTotales%25 == 0)   
            {
                if (!ganoVidas) //Limita las vidas ganadas cada 25 monedas a 1.
                {
                    ganoVidas = true;
                    ship.vidas++;
                }
            }
            else if(ganoVidas)  
            {
                ganoVidas = false;
            }

            //Envia a la pantalla final si se acaban las vidas
            if (ship.vidas <= 0)
            {
                Game1.INSTANCE.ChangeScene(Game1.Scene.End);
            }

        }
        public void Draw (SpriteBatch SB)
        {
            Vector2 timerPos;
            Vector2 vidasPos;
            Vector2 powerUpPos;
            Vector2 scorePos;

            timerPos = new Vector2(50, 25);
            vidasPos = new Vector2((Game1.INSTANCE.GraphicsDevice.Viewport.Width) - 100,25);
            powerUpPos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 4f, 25);
            scorePos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2f, 25);

            SB.Draw(timerIcon, timerPos, null, Color.White, 0f, new Vector2(0, 0), new Vector2(0.05f,0.05f), SpriteEffects.None, 1f);
            SB.Draw(vidasIcon, vidasPos, null, Color.White, 0f, new Vector2(0, 0), new Vector2(0.05f, 0.05f), SpriteEffects.None, 1f);
            SB.Draw(powerUpIcon, powerUpPos, null, Color.White, 0f, new Vector2(0, 0), new Vector2(0.05f, 0.05f), SpriteEffects.None, 1f);

            SB.DrawString(timer, "    " + Math.Round(time,2), timerPos, Color.White);
            SB.DrawString(vidasActuales, "     x " + Game1.INSTANCE.ventanaJuego.ship.vidas, vidasPos, Color.White);
            SB.DrawString(powerUpLevel, "    Energy lvl: " + Game1.INSTANCE.ventanaJuego.ship.buffLevel, powerUpPos, Color.White);
            SB.DrawString(puntajeTotal, "    Score: " + Game1.INSTANCE.ventanaJuego.score, scorePos, Color.White);

            if (paused)
            {
                Vector2 tituloPos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2.33f, 100);
                Vector2 resumePos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2.8f, 200);
                Vector2 restartPos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2.8f, 300);
                Vector2 mainMenuPos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2.8f, 400);
                Vector2 exitPos = new Vector2(Game1.INSTANCE.GraphicsDevice.Viewport.Width / 2.8f, 500);

                SB.Draw(fondoPausa, new Vector2(0, 0), Color.White * 0.7f);

                SB.DrawString(timer, " Pausa ", tituloPos, Color.White);
                SB.DrawString(powerUpLevel, " [P]-> Volver al juego\n", resumePos, Color.White);
                SB.DrawString(powerUpLevel, " [R]-> Reiniciar Juego\n", restartPos, Color.White);
                SB.DrawString(powerUpLevel, " [M]-> Volver al menu principal.\n", mainMenuPos, Color.White);
                SB.DrawString(powerUpLevel, " [Esc]-> Cerrar ventana de juego.\n ", exitPos, Color.White);
            }
        }

    }
}
