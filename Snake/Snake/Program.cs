using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Snake
{
    class Program
    {
        enum Direccion
        {
            Arriba,
            Abajo,
            Izquierda,
            Derecha         
        }
        class Punto
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Punto(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            public Punto() { }
        }
        static int Score;
        static int ScoreMax;
        static string ScoreMaximoName;
        static bool CanMove = true;
        static Direccion dir = Direccion.Izquierda;
        static bool Juega = true;
        static List<Punto> Serpiente = new List<Punto>();
        static Punto PosicionComida = new Punto();//punto d la comida
        static void Main(string[] args)
        {
            Console.Title = "Snake by Fadek";
            LeerPuntajeMax();
            Menu();
        }

        private static void LeerPuntajeMax()
        {
            if(File.Exists("score.txt"))
            {
                string[] text = File.ReadAllLines("score.txt");

                if(text.Length>=2)
                {
                    ScoreMax = Convert.ToInt32(text[0]);
                    ScoreMaximoName = text[1];
                }
            }
        }

        private static void Menu()
        {
            string Tecla="\0";
            while(Tecla.ToUpper() != "S")
            {
                Console.Clear();
                Console.WriteLine("Bienvenido a Snake, hecho por fadek\n\n\n");
                Console.WriteLine("(J)ugar");
                Console.WriteLine("(P)untaje");
                Console.WriteLine("(S)alir\n");
                Tecla = Console.ReadLine();
                if(Tecla.ToUpper()== "J")
                {
                    Jugar();
                }
                else if(Tecla.ToUpper()=="P")
                {
                    Puntaje();
                }
            }
        }

        private static void Puntaje()
        {
            Console.Clear();
            if(ScoreMax!=0)
            {
                Console.WriteLine($"Puntaje Maximo: {ScoreMax }\nPor: { ScoreMaximoName}");
            }
            else
            {
                Console.WriteLine("No existe ningun puntaje :(");
            }
            
            Console.ReadKey();

        }

        private static void Jugar()
        {
            Console.Clear();
            SpawnComida();
            Score = 0;
            Juega = true;
            dir = Direccion.Izquierda;
            Serpiente = new List<Punto>()
            {
                new Punto( Console.WindowWidth /2,Console.WindowHeight /2)
            };

            Thread threadteclas = new Thread(DetectarTeclas);
            threadteclas.SetApartmentState(ApartmentState.STA);
            threadteclas.Start();

            ActualizaPuntaje();

            while (Juega)
            {
                Mover();
                Thread.Sleep(100);
            }

        }

        private static void ActualizaPuntaje()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Score: " + Score);
        }

        private static void SpawnComida()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());//sirve para que no se genere los mismos nunmeros aleatorios todo el tiempo
            
            int x, y;
            x = random.Next(0, Console.WindowWidth - 1);// para que este menos del ancho de la ventana
            y = random.Next(0, Console.WindowHeight - 1);//menos del alto
            while (Serpiente.Where(n =>n.X == x && n.Y == y).Any())//si existe algun elemento dentro de mi lista serpiente donde n x == al x que yo cree, como en el y, es decir si ya existe esa posicion, la comida se vuelve a generar en otro lado
            {
                x = random.Next(0, Console.WindowWidth - 1);
                y = random.Next(0, Console.WindowHeight - 1);
            }
            PosicionComida = new Punto(x, y);
            Console.SetCursorPosition(x,y);
            Console.Write(".");
        }

        private static void DetectarTeclas()
        {
            while (Juega)
            {
                if (CanMove)
                { 
                    if (dir != Direccion.Abajo && Keyboard.IsKeyDown(Key.Up))
                    {
                        dir = Direccion.Arriba;
                    }
                    else if (dir != Direccion.Arriba && Keyboard.IsKeyDown(Key.Down))
                    {
                        dir = Direccion.Abajo;
                    }
                    else if (dir != Direccion.Izquierda && Keyboard.IsKeyDown(Key.Right))
                    {
                        dir = Direccion.Derecha;
                    }
                    else if (dir != Direccion.Derecha && Keyboard.IsKeyDown(Key.Left))
                    {
                        dir = Direccion.Izquierda;
                    }
                }
            }
        }

        private static void Mover()
        {
            Punto PosionAux = null;
            CanMove = false;
            for(int i=0; i<Serpiente.Count();i++)
            {
                Console.SetCursorPosition(Serpiente[i].X, Serpiente[i].Y);
                Console.Write(" ");
                if (i == 0)
                {
                    PosionAux = new Punto(Serpiente[0].X, Serpiente[0].Y);
                    if (dir == Direccion.Arriba)
                    {
                        Serpiente[i].Y -= 1;
                    }
                    else if (dir == Direccion.Abajo)
                    {
                        Serpiente[i].Y += 1;
                    }
                    else if (dir == Direccion.Izquierda)
                    {
                        Serpiente[i].X -= 1;
                    }
                    else if (dir == Direccion.Derecha)
                    {
                        Serpiente[i].X += 1;
                    }  
                }
                else 
                {
                    var PosicionAux2 = new Punto(PosionAux.X, PosionAux.Y);
                    PosionAux = new Punto(Serpiente[i].X, Serpiente[i].Y);
                    Serpiente[i] = new Punto(PosicionAux2.X, PosicionAux2.Y);
                }
                Console.SetCursorPosition(Serpiente[i].X, Serpiente[i].Y);
                Console.Write("o");

            }
            CanMove = true;//para que el usuario no empiece a tocar letras y cambie todo
            DetectarColisiones();
        }

        private static void DetectarColisiones()
        {
            var CabezaSerpiente = Serpiente.First();
            if(CabezaSerpiente.X==PosicionComida.X && CabezaSerpiente.Y==PosicionComida.Y)
            {
                Score += 10;
                ActualizaPuntaje();
                if(Serpiente.Count==1)
                {
                    if(dir == Direccion.Arriba)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X, CabezaSerpiente.Y + 1));
                    }
                    else if (dir == Direccion.Abajo)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X, CabezaSerpiente.Y - 1));
                    }
                    else if (dir == Direccion.Izquierda)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X +1, CabezaSerpiente.Y));
                    }
                    else if (dir == Direccion.Derecha)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X -1, CabezaSerpiente.Y));
                    }
                }
                else
                {
                    var Ultimo = Serpiente.Last();
                    var AnteUltimo = Serpiente[Serpiente.Count - 2];
                    //si esta llendo desde hacia abajo
                    if(Ultimo.X == AnteUltimo.X && Ultimo.Y+1 == AnteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X, CabezaSerpiente.Y - 1));
                    }
                    //arriba
                    else if (Ultimo.X == AnteUltimo.X && Ultimo.Y - 1 == AnteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X, CabezaSerpiente.Y +1));
                    }
                    //izquierda
                    else if (Ultimo.X - 1 == AnteUltimo.X && Ultimo.Y == AnteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X + 1, CabezaSerpiente.Y));
                    }
                    //derecha
                    else if (Ultimo.X +1 == AnteUltimo.X && Ultimo.Y == AnteUltimo.Y)
                    {
                        Serpiente.Add(new Punto(CabezaSerpiente.X - 1, CabezaSerpiente.Y));
                    }
                }
                PosicionComida = null;
                SpawnComida();
            }
            if(Serpiente.Where(n=>n.X == Serpiente[0].X && n.Y == Serpiente[0].Y && !n.Equals(Serpiente[0])).Any())
            {
                Perder();
            }
            if(Serpiente[0].X <= 0 || Serpiente[0].X >= Console.WindowWidth -1 || Serpiente[0].Y <= 1 || Serpiente[0].Y >= Console.WindowHeight -1 )
            {
                Perder();
            }
        }

        private static void Perder()
        {
            Juega = false;
            Console.SetCursorPosition(10,10);
            Console.Write("PERDISTE");
            Console.ReadKey();
            if(Score>ScoreMax)
            {
                Console.Clear();
                Console.WriteLine($"supero el puntaje maximo con { Score }\nIngrese su nombre para quedar registrado: ");
                ScoreMaximoName = Console.ReadLine();
                ScoreMax = Score;
                GuardarPuntajeMax();
            }
        }

        private static void GuardarPuntajeMax()
        {
            string Texto = ScoreMax + "\n" + ScoreMaximoName;
            File.WriteAllText("score.txt", Texto);
        }
    }
}
