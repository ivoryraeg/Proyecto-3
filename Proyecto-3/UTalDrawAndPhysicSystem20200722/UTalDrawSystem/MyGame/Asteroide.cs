using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTalDrawSystem.SistemaAudio;
using UTalDrawSystem.SistemaGameObject;

namespace UTalDrawSystem.MyGame
{
    class Asteroide : UTGameObject
    {
        
        public int hp;

        Vector2 velActual;       

        public Asteroide(string imagen, Vector2 pos, float escala, FF_form forma, bool isStatic = false, bool isSuperior = true) : base(imagen, pos, escala, forma, isStatic, isSuperior = true)
        {
            hp = 5;

            velActual = new Vector2(0);
        }

        public override void Update(GameTime gameTime)
        {
            if (hp <= 0)
            {
                Game1.INSTANCE.ventanaJuego.score  += 100;
                AudioManager.Play(AudioManager.Sounds.Pop, true);
                Destroy();
            }

            if (Game1.INSTANCE.ventanaJuego.paused)
            {
                if (velActual == new Vector2 (0))
                {
                    velActual = objetoFisico.vel;
                }
                objetoFisico.SetVelocity(new Vector2 (0));
            }
            else
            {
                objetoFisico.AddVelocity(velActual);
                velActual = new Vector2(0);
            }
        }
        /*
        public override void OnCollision(UTGameObject other)
        {
            Asteroide ast = other as Asteroide;

            if (ast != null)
            {
                //ast.objetoFisico.SetVelocity(objetoFisico.vel / 1);
            }
        }
        */
    }
}
