using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WinD = System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using sRectangle = Microsoft.Xna.Framework.Rectangle;
using static System.Math;
using static Euristics.Core;

namespace Euristics
{
    public class Gene
    {
        internal float[] keyvalues;
        public float[] GetKeyvalues => keyvalues;

        public Gene(params float[] genecode)
        {
            keyvalues = genecode;
        }

        public void Update(Action a)
        {
            a.Invoke();
        }
    }

    public struct ResultEntry
    {
        public float[] data;
        public int gens;
        public Color col;
        public ResultEntry(Generation g)
        {
            gens = g.gen;
            data = g.genes[0].keyvalues;
            col = new Color(data[0] / 100, data[1] / 100, data[2] / 100);
        }
    }

    public class Generation
    {
        internal List<Gene> genes;

        public Generation(List<Gene> genes)
        {
            this.genes = genes;
        }

        internal int gen;
        public int Step => gen;

        bool restart;
        public bool CanRestart { get => restart; internal set => restart = value; }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Deferred);
            {
                batch.DrawString(font, $"Generation: {gen}", new Vector2(245, 70), Color.White);
                int i = 0, j = 0;
                foreach (var g in genes)
                {
                    if (i > 20)
                    {
                        j += 1;
                        i = 0;
                    }
                    batch.DrawFill(new Rectangle(245 + 25 * i, 100 + 30 * j, 20, 20), new Color(g.keyvalues[0] / 100, g.keyvalues[1] / 100, g.keyvalues[2] / 100));
                    i++;
                }
            }
            batch.End();
        }
    }

    public static class Genetics
    {
        public static Gene SPC(Gene parent1, Gene parent2)
        {
            float[] crossbuffer1 = parent1.GetKeyvalues;
            float[] crossbuffer2 = parent2.GetKeyvalues;

            for (int i = 0; i < crossbuffer1.Length; i++)
            {
                float sp = Rand;

                crossbuffer1[i] = crossbuffer1[i] * (sp + 0.05f) + crossbuffer2[i] * (1 - (sp + 0.05f));
            }

            return new Gene(crossbuffer2);
        }

        public static void Evolve(Generation pop, Func<float> f)
        {
            if (pop.genes.Count > MaxGenes || pop.genes.Count == 0)
            {
                pop.CanRestart = true;
                return;
            }

            foreach (var g in pop.genes)
            {
                g.Update(() => { g.keyvalues[3] = g.keyvalues[0] / g.keyvalues[2] / g.keyvalues[1]; });
            }

            var pre = pop.genes.FindAll(n =>
            {
                // Volatility
                float res = Rand * n.keyvalues[3];
                return res > f();
            });

            pop.genes = pre;
            var gc = pop.genes.Count;
            for (int i = 0; i < gc; i++)
            {

                pop.genes.Add(SPC(pre[(int)(Rand * gc)], pre[(int)(Rand * gc)]));
            }
            pop.gen++;
        }

        public static void Evolve(List<Gene> pop, params float[] cond)
        {
            //pop.Find(n => 
            //{
            //    float res = 0;
            //    for (int i = 0; i < cond.Length; i++)
            //    {
            //        res += n.keyvalues[i] > cond[i] ? n.keyvalues[i] : n.keyvalues[i] * 0.5f;
            //    }
            //    return 
            //});
        }
    }
}
