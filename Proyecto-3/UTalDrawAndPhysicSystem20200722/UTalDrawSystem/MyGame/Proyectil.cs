using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using UTalDrawSystem.SistemaAudio;
using UTalDrawSystem.SistemaGameObject;
using static UTalDrawSystem.SistemaAudio.AudioManager;

namespace UTalDrawSystem.MyGame
{
    class Proyectil : UTGameObject
    {
        float vel_X;
        float vel_Y;
        public bool isDestroyed;

        public Proyectil(string imagen, Vector2 pos, float velX, float velY, float rot, float escala, FF_form forma, bool isStatic = false, bool isSuperior = false) : base(imagen, pos, escala, forma, isStatic, isSuperior)
        {
            vel_X = velX;
            vel_Y = velY;
            objetoFisico.dibujable.rot = rot;
            isDestroyed = false;
            objetoFisico.isTrigger = true;
        }

        public override void Update(GameTime gameTime)
        {
            if(!Game1.INSTANCE.ventanaJuego.paused)
            {
                objetoFisico.pos += new Vector2(vel_X, vel_Y);
            }

        }

        public override void OnCollision(UTGameObject other)
        {
            Asteroide ast = other as Asteroide;

            if (ast != null)
            {
                AudioManager.Play(AudioManager.Sounds.Hitmarker, true);
                ast.hp -= 1;

                //Console.WriteLine(ast.hp);
                isDestroyed = true;
                Destroy();
            }
        }
    }
}
