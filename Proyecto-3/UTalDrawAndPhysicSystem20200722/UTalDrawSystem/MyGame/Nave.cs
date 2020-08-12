﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UTalDrawSystem.SistemaAudio;
using UTalDrawSystem.SistemaGameObject;

namespace UTalDrawSystem.MyGame
{
    public class Nave : UTGameObject
    {
        UTGameObject shield;

        public int vidas;
        public int nuevaVida;
        public int powerUpTotales = 0;

        public bool invulnerable;
        public float tiempoInvulnerable;

        public float shootCD;
        public bool isShooting;

        public int buffLevel;

        Vector2 respawnPos;

        public Nave(ContentManager content, string imagen, Vector2 pos, float escala, FF_form forma, bool isStatic = false, bool isSuperior = true) : base(imagen, pos, escala, forma, isStatic, isSuperior)
        {
            vidas = 50000000;
            nuevaVida = 5000;
            respawnPos = pos;

            invulnerable = false;
            tiempoInvulnerable = 0;

            shootCD = 0f;
            isShooting = false;

            buffLevel = 1;

            objetoFisico.dibujable.rot = 1.57f;
        }
        public override void Update(GameTime gameTime)
        {
            float vel = 100;
            // 1.57 90 grados 
            // 3.14 180 grados 
            // 4.71 240 grados 
            // 6.28 360 grados
            
            if (Game1.INSTANCE.ventanaJuego.paused)
            {
                objetoFisico.SetVelocity(new Vector2(0));
            }


            if (!Game1.INSTANCE.ventanaJuego.paused)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    objetoFisico.AddVelocity(new Vector2((float)gameTime.ElapsedGameTime.TotalSeconds * vel, 0));
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    objetoFisico.AddVelocity(new Vector2(-(float)gameTime.ElapsedGameTime.TotalSeconds * vel, 0));
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    objetoFisico.AddVelocity(new Vector2(0, -(float)gameTime.ElapsedGameTime.TotalSeconds * vel));
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    objetoFisico.AddVelocity(new Vector2(0, (float)gameTime.ElapsedGameTime.TotalSeconds * vel));
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    isShooting = true;
                }
                if (shootCD > 0f)
                {
                    shootCD -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            

            if (invulnerable)
            {
                shield.objetoFisico.pos = objetoFisico.pos;

                if (tiempoInvulnerable > 3)
                {
                    shield.Destroy();
                    tiempoInvulnerable = 0;
                    invulnerable = false;
                    objetoFisico.isTrigger = false;
                }
                else
                {
                    if (!Game1.INSTANCE.ventanaJuego.paused)
                    {
                        tiempoInvulnerable += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
        }

        public override void OnCollision(UTGameObject other)
        {
            Coleccionable col = other as Coleccionable; 
            Agujero obs = other as Agujero;
            Asteroide ball = other as Asteroide;

            if (col != null)
            {
                col.Destroy();
                powerUpTotales++;
                Game1.INSTANCE.ventanaJuego.score += 1000;

                AudioManager.Play(AudioManager.Sounds.Slurp);
                if (buffLevel < 5)
                {
                    buffLevel++;
                }
                //Console.WriteLine(powerUpTotales);
            }
            if (obs != null)
            {
                if (!invulnerable)
                {
                    invulnerable = true;
                    objetoFisico.pos = Respawn();
                }
            }
            if (ball != null)
            {
                if (!invulnerable)
                {
                    vidas--;
                    ball.hp--;
                }
            }
        }

        public Vector2 Respawn()
        {
            vidas--;
            respawnPos.X = Game1.INSTANCE.ventanaJuego.camara.pos.X + Game1.INSTANCE.GraphicsDevice.Viewport.Width/2f;
            respawnPos.Y = Game1.INSTANCE.ventanaJuego.camara.pos.Y + Game1.INSTANCE.GraphicsDevice.Viewport.Height;

            objetoFisico.isTrigger = true;
            shield = new UTGameObject("energyShield", objetoFisico.pos, 0.2f, FF_form.Circulo, false, true);
            shield.objetoFisico.isTrigger = true;
            buffLevel = 1;

            return respawnPos;
        }
    }
}
