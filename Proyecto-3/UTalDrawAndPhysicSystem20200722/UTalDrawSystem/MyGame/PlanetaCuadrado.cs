using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using UTalDrawSystem.SistemaGameObject;

namespace UTalDrawSystem.MyGame
{
    class PlanetaCuadrado : UTGameObject
    {
        public PlanetaCuadrado(string imagen, Vector2 pos, float escala, FF_form forma, bool isStatic = false, bool isSuperior = false, bool isInferior = false) : base(imagen, pos, escala, forma, isStatic, isSuperior, isInferior)
        {

        }

        public override void OnCollision(UTGameObject other)
        {
            Asteroide ast = other as Asteroide;
            Agujero hole = other as Agujero;
            Coleccionable col = other as Coleccionable;

            if (objetoFisico.pos.X > Game1.INSTANCE.ventanaJuego.camara.pos.X + 225 + Game1.INSTANCE.GraphicsDevice.Viewport.Width * 2)
            {
                if (ast != null)
                {
                    ast.Destroy();
                }
                if (hole != null)
                {
                    hole.Destroy();
                }
                if (col != null)
                {
                    col.Destroy();
                }
            }
        }
    }
}
