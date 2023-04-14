using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BGLeopold
{
    public struct Vector2
    {
        public static Vector2 zero = new Vector2(0f, 0f);
        public static Vector2 one = new Vector2(1f, 1f);
        public static Vector2 right = new Vector2(1f, 0f);
        public static Vector2 left = new Vector2(-1f, 0f);
        public static Vector2 up = new Vector2(0f, 1f);
        public static Vector2 down = new Vector2(0f, -1f);

        public static float Dot(in Vector2 a, in Vector2 b) => a.x * b.x + a.y * b.y;

        public float x, y;
        public Vector2(in float x)
        {
            this.x = x;
            y = 0f;
        }
        public Vector2(in float x, in float y)
        {
            this.x = x;
            this.y = y;
        }
        public float SqrMagnitude() => x * x + y * y;
        public float Magnitude() => (float)Math.Sqrt(x * x + y * y);
        public void Normalize()
        {
            this = this * (1f / Magnitude());
        }

        public static Vector2 operator+(in Vector2 a) => a;
        public static Vector2 operator-(in Vector2 a) => new Vector2(-a.x, -a.y);
        public static Vector2 operator+(in Vector2 a, in Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator-(in Vector2 a, in Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator*(in Vector2 a, in float b) => new Vector2(b * a.x, b * a.y);
        public static Vector2 operator*(in float b, in Vector2 a) => new Vector2(b * a.x, b * a.y);
        public static Vector2 operator/(in Vector2 a, in float b) => new Vector2(a.x / b, a.y / b);
    }
    public struct Vector3
    {
        public static Vector3 zero = new Vector3();
        public static Vector3 one = new Vector3(1f, 1f, 1f);

        public static float Dot(in Vector3 a, in Vector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        public float x, y, z;
        public Vector3(in float x)
        {
            this.x = x;
            y = z = 0f;
        }
        public Vector3(in float x, in float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
        }
        public Vector3(in float x, in float y, in float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float SqrMagnitude() => x * x + y * y + z * z;
        public float Magnitude() => (float)Math.Sqrt(x * x + y * y + z * z);
        public void Normalize()
        {
            this = this * (1f / Magnitude());
        }
        public static Vector3 operator +(in Vector3 a) => a;
        public static Vector3 operator -(in Vector3 a) => new Vector3(-a.x, -a.y, -a.z);
        public static Vector3 operator +(in Vector3 a, in Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator -(in Vector3 a, in Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator *(in Vector3 a, in float b) => new Vector3(b * a.x, b * a.y, b * a.z);
        public static Vector3 operator *(in float b, in Vector3 a) => new Vector3(b * a.x, b * a.y, b * a.z);
        public static Vector3 operator /(in Vector3 a, in float b) => new Vector3(a.x / b, a.y / b, a.z / b);
    }

    public delegate string SerialyseFunction<T>(T obj);
    public delegate T DeserialyseFunction<T>(string s);
    public delegate float Function(in float x);

    public static class Random
    {
        private static System.Random random = new System.Random();
        public static void SetRandomSeed(in int seed)
        {
            random = new System.Random(seed);
            Noise2d.Reseed();
        }
        /// <summary>
        /// randomize de seed of the random, allow to have diffenrent random number at each lauch of the game
        /// </summary>
        public static void SetRandomSeed()
        {
            SetRandomSeed((int)DateTime.Now.Ticks);
        }
        /// <returns> A random integer between a and b, [|a, b|]</returns>
        public static int Rand(in int a, in int b) => random.Next(a, b + 1);
        /// <returns> A random float between 0 and 1, [0, 1]</returns>
        public static float Rand() => (float)random.Next(int.MaxValue) / (int.MaxValue - 1);
        /// <returns> A random float between a and b, [a, b]</returns>
        public static float Rand(in float a, in float b) => Rand() * (float)Math.Abs(b - a) + a;
        /// <returns> A random int between a and b, [|a, b|[</returns>
        public static int RandExclude(in int a, in int b) => random.Next(a, b);
        /// <returns> A random double between a and b, [a, b[</returns>
        public static float RandExclude(in float a, in float b) => (float)random.NextDouble() * ((float)Math.Abs(b - a)) + a;
        public static float RandExclude() => (float)random.NextDouble();
        public static float PerlinNoise(in float x, in float y) => Noise2d.Noise(x, y);

        public static Vector2 RandomVector2()
        {
            float angle = RandExclude(0f, 2f * (float)Math.PI);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        /// <returns> A random Vector2 with de magnitude in param</returns>
        public static Vector2 RandomVector2(in float magnitude)
        {
            float angle = RandExclude(0f, 2f * (float)Math.PI);
            return new Vector2(magnitude * (float)Math.Cos(angle), magnitude * (float)Math.Sin(angle));
        }
        /// <returns> A random Vector2 with a randoml magnitude</returns>
        public static Vector2 RandomVector2(in float minMagnitude, in float maxMagnitude)
        {
            float angle = RandExclude(0f, 2f * (float)Math.PI);
            float magnitude = Rand(minMagnitude, maxMagnitude);
            return new Vector2(magnitude * (float)Math.Cos(angle), magnitude * (float)Math.Sin(angle));
        }
        /// <returns> A random Vector3 normalised</returns>
        public static Vector3 RandomVector3()
        {
            float teta = Rand(0f, (float)Math.PI);
            float phi = RandExclude(0f, 2f * (float)Math.PI);
            return new Vector3((float)Math.Sin(teta) * (float)Math.Cos(phi), (float)Math.Sin(teta) * (float)Math.Sin(phi), (float)Math.Cos(teta));
        }
        /// <returns> A random Vector3 with de magnitude in param</returns>
        public static Vector3 RandomVector3(in float magnitude)
        {
            float teta = Rand(0f, (float)Math.PI);
            float phi = RandExclude(0f, 2f * (float)Math.PI);
            return new Vector3(magnitude * (float)Math.Sin(teta) * (float)Math.Cos(phi), magnitude * (float)Math.Sin(teta) * (float)Math.Sin(phi), magnitude * (float)Math.Cos(teta));
        }
        /// <returns> A random Vector3 with a random magnitude</returns>
        public static Vector3 RandomVector3(in float minMagnitude, in float maxMagnitude)
        {
            float teta = Rand(0f, (float)Math.PI);
            float phi = RandExclude(0f, 2f * (float)Math.PI);
            float magnitude = Rand(minMagnitude, maxMagnitude);
            return new Vector3(magnitude * (float)Math.Sin(teta) * (float)Math.Cos(phi), magnitude * (float)Math.Sin(teta) * (float)Math.Sin(phi), magnitude * (float)Math.Cos(teta));
        }
        private static class Noise2d
        {
            private static int[] _permutation;

            private static Vector2[] _gradients;

            static Noise2d()
            {
                CalculatePermutation(out _permutation);
                CalculateGradients(out _gradients);
            }

            private static void CalculatePermutation(out int[] p)
            {
                p = Enumerable.Range(0, 256).ToArray();

                /// shuffle the array
                for (var i = 0; i < p.Length; i++)
                {
                    var source = RandExclude(0, p.Length);

                    var t = p[i];
                    p[i] = p[source];
                    p[source] = t;
                }
            }

            /// <summary>
            /// generate a new permutation.
            /// </summary>
            public static void Reseed()
            {
                CalculatePermutation(out _permutation);
            }

            private static void CalculateGradients(out Vector2[] grad)
            {
                grad = new Vector2[256];

                for (var i = 0; i < grad.Length; i++)
                {
                    Vector2 gradient;
                    do
                    {
                        gradient = new Vector2((RandExclude() * 2f - 1f), (RandExclude() * 2f - 1f));
                    }
                    while (gradient.SqrMagnitude() >= 1);

                    gradient.Normalize();

                    grad[i] = gradient;
                }
            }

            private static float Drop(float t)
            {
                t = (float)Math.Abs(t);
                return 1f - t * t * t * (t * (t * 6 - 15) + 10);
            }

            private static float Q(in float u, in float v)
            {
                return Drop(u) * Drop(v);
            }

            public static float Noise(in float x, in float y)
            {
                var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

                var total = 0f;

                var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

                foreach (var n in corners)
                {
                    var ij = cell + n;
                    var uv = new Vector2(x - ij.x, y - ij.y);

                    var index = _permutation[(int)ij.x % _permutation.Length];
                    index = _permutation[(index + (int)ij.y) % _permutation.Length];

                    var grad = _gradients[index % _gradients.Length];

                    total += Q(uv.x, uv.y) * Vector2.Dot(grad, uv);
                }
                return (float)Math.Max((float)Math.Min(total, 1f), -1f);
            }
        }

        //Generer les lois de probas (je les ai jamais tester mdrrr
        public static int Bernoulli(in float p) => Rand() <= p ? 1 : 0;
        public static int Binomial(in int n, in int p)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
                count += Bernoulli(p);
            return count;
        }
        public static float Expodential(in float lambda) => (-1f / lambda) * (float)Math.Log(Rand());
        public static int Poisson(in float lambda)
        {
            float x = Rand();
            int n = 0;
            while (x > (float)Math.Exp(-lambda))
            {
                x *= Rand();
                n++;
            }
            return n;
        }
        public static int Geometric(in float p)
        {
            int count = 0;
            do
            {
                count++;
            } while (Bernoulli(p) == 0);
            return count;
        }
        private static float N01() => (float)Math.Sqrt(-2f * (float)Math.Log(Rand())) * (float)Math.Cos(2f * (float)Math.PI * Rand());
        public static float Normal(in float esp, in float sigma) => N01() * sigma + esp;
    }

    public static class Useful
    {
        #region Vector 

        //Vector2
        public static float SqrDistance(this Vector2 v, in Vector2 a) => (a.x - v.x) * (a.x - v.x) + (a.y - v.y) * (a.y - v.y);
        public static float Distance(this Vector2 v, in Vector2 a) => (float)Math.Sqrt(v.SqrDistance(a));
        /// <summary>
        /// Le produit scalaire
        /// </summary>
        public static float DotProduct(this Vector2 v, in Vector2 a) => v.x * a.x + v.y * a.y;
        /// <summary>
        /// Le produit vectorielle
        /// </summary>
        public static Vector3 CrossProduct(this Vector2 a, in Vector2 b) => new Vector3(0f, 0f, a.x * b.y - a.y * b.x);
        public static bool IsCollinear(this Vector2 a, in Vector2 b) => (float)Math.Abs((b.x / a.x) - (b.y / a.y)) < 0.007f * (float)Math.Abs(b.y / a.y);

        //Vector3
        public static float SqrDistance(this Vector3 v, in Vector3 a) => (a.x - v.x) * (a.x - v.x) + (a.y - v.y) * (a.y - v.y) + (a.z - v.z) * (a.z - v.z);
        public static float Distance(this Vector3 v, in Vector3 a) => (float)Math.Sqrt(v.SqrDistance(a));
        /// <summary>
        /// Le produit scalaire
        /// </summary>
        public static float DotProduct(this Vector3 v, in Vector3 a) => v.x * a.x + v.y * a.y + v.z * a.z;
        /// <summary>
        /// Le produit vectorielle
        /// </summary>
        public static Vector3 CrossProduct(this Vector3 a, in Vector3 b) => new Vector3(a.y * b.z - a.z * b.y, -a.x * b.z + a.z * b.x, a.x * b.y - a.y * b.x);
        public static bool IsCollinear(this Vector3 a, in Vector3 b) => (float)Math.Abs(b.x / a.x - b.y / a.y) < 0.007f * (float)Math.Abs(b.y / a.y) &&
                                                                        (float)Math.Abs(b.x / a.x - b.z / a.z) < 0.007f * (float)Math.Abs(b.z / a.z) &&
                                                                        (float)Math.Abs(b.y / a.y - b.z / a.z) < 0.007f * (float)Math.Abs(b.z / a.z);
           
        /// <summary>
        /// Renvoie un vecteur orthogonal à ce vecteur
        /// </summary>
        public static Vector2 NormalVector(this Vector2 v)
        {
            if((float)Math.Abs(v.y) > 0.001f)
            {
                return new Vector2(10f, (-v.x * 10f) / v.y);
            }
            if ((float)Math.Abs(v.x) > 0.001f)
            {
                return new Vector2((-v.y * 10f) / v.x, 10f);
            }
            return Vector2.zero;
        }

        #endregion

        #region Angle

        public static float ToRad(float angle) => (float)(((angle * (float)Math.PI) / 180f) % (2 * (float)Math.PI));
        public static float ToDegrees(float angle) => (float)(((angle * 180f) / (float)Math.PI) % 360);

        /// <returns> l'angle entre les vecteurs (1, 0) et (pos2 - pos1) compris entre 0 et 2pi radian</returns>
        public static float Angle(in Vector2 pos1, in Vector2 pos2) => (float)Math.Atan2(pos1.y - pos2.y, pos1.x - pos2.x) + (float)Math.PI;
        /// <summary>
        /// Renvoie l'angle minimal (pos1, center, pos2)
        /// </summary>
        public static float Angle(in Vector2 center, in Vector2 pos1, in Vector2 pos2)
        {
            float ang1 = Angle(center, pos1);
            float ang2 = Angle(center, pos2);
            float diff = (float)Math.Abs(ang1 - ang2);
            return (float)Math.Min(diff, 2f * (float)Math.PI - diff);
        }
        /// <summary>
        /// Renvoie si pour aller de l'angle 1 vers l'angle 2 le plus rapidement il faut tourner à droite au à gauche
        /// </summary>
        public static void DirectionAngle(float ang1, float ang2, out bool right)
        {
            WrapAngle(ang1);
            WrapAngle(ang2);
            float diff = (float)Math.Abs(ang1 - ang2);
            float angMin = (float)Math.Min(diff, 2f * (float)Math.PI - diff);
            right = (float)Math.Abs((ang1 + angMin) % (2f * (float)Math.PI) - ang2) < 0.1f;
        }
        /// <summary>
        /// Renvoie l'angle en radian égal à l'angle en param mais dans [0, 2π[
        /// </summary>
        public static float WrapAngle(in float angle) => Clamp(0f, 2f * (float)Math.PI, angle);
        /// <returns> value if value is in the range [a, b], a or b otherwise</returns>
        public static float MarkOut(in float min, in float max, in float value) => (float)Math.Max((float)Math.Min(value, max), min);

        /// <returns> a like a = value % (end -  start) + start, a€[start, end[ /returns>
        public static float Clamp(in float start, in float end, in float value)
        {
            if (end < start)
                return Clamp(end, start, value);
            if (end == start)
                return start;

            if (value < end && value >= start)
                return value;
            else
            {
                float modulo = end - start;
                float result = (value % modulo) + start;
                if (result >= end)
                    return result - modulo;
                if (result < start)
                    return result + modulo;
                return result;
            }
        }

        #endregion

        #region Lerp

        /// <returns>The linear interpolation between start and end, t€[0f, 1f]</returns>
        public static float Lerp(in float start, in float end, in float t) => (end - start) * t + start;
        /// <returns>The linear interpolation between start and end, t€[0f, 1f]</returns>

        #endregion

        /// <summary>
        /// Impair
        /// </summary>
        public static bool IsOdd(in int number) => number % 2 == 1;
        /// <summary>
        /// Pair
        /// </summary>
        public static bool IsEven(int number) => number % 2 == 0;
        
        /// <summary>
        /// Renvoie la valeur arrondi de n
        /// </summary>
        public static int Round(in float n) => (n - (float)Math.Floor(n)) >= 0.5f ? (int)n + 1 : (int)n;
        /// <summary>
        /// Renvoie la valeur arrondi de n au nombre de décimales en param, ex : Round(51.6854, 2) => 51.69
        /// </summary>
        public static float Round(in float n, in int nbDecimals)
        {
            float npow = n * (float)Math.Pow(10, nbDecimals);
            return npow - (int)(npow) >= 0.5f ? (((int)(npow + 1)) / (float)Math.Pow(10, nbDecimals)) : (((int)npow) / (float)Math.Pow(10, nbDecimals));
        }

        #region CloneArray

        public static T[,] Clone<T>(this T[,] Array)
        {
            T[,] a = new T[Array.GetLength(0), Array.GetLength(1)];
            for (int l = 0; l < a.GetLength(0); l++)
            {
                for (int c = 0; c < a.GetLength(1); c++)
                {
                    a[l, c] = Array[l, c];
                }
            }
            return a;
        }

        #endregion

        #region GetSubArray
        /// <summary>
        /// Retourne le sous tableau de Array, cad Array[IndexStart]
        /// </summary>
        /// <param name="indexStart">l'index de la première dimension de Array</param>
        public static T[,,] GetSubArray<T>(this T[,,,] Array, in int indexStart = 0)
        {
            T[,,] a = new T[Array.GetLength(1), Array.GetLength(2), Array.GetLength(3)];
            for (int l = 0; l < a.GetLength(0); l++)
            {
                for (int c = 0; c < a.GetLength(1); c++)
                {
                    for (int i = 0; i < a.GetLength(2); i++)
                    {
                        a[l, c, i] = Array[indexStart, l, c, i];
                    }
                }
            }
            return a;
        }
        #endregion

        public static bool CaseExistArray<T>(in T[,] tab, int l, int c) => l >= 0 && c >= 0 && l < tab.GetLength(0) && c < tab.GetLength(1);

        #region Affiche Array    

        public static void Show<T>(this T[] tab)
        {
            string text = "[ ";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += tab[l].ToString() + ", ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ]";
            Debug.WriteLine(text);
        }
        public static void ShowArray<T>(this T[,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text = "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += tab[l, c].ToString() + ", ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ],";
                Debug.WriteLine(text);
            }
        }
        public static void ShowArray<T>(this T[,,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += "[ ";
                    for (int i = 0; i < tab.GetLength(2); i++)
                    {
                        text += tab[l, c, i].ToString() + ", ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 3, 3);
            text += "]";
            Debug.WriteLine(text);
        }
        public static void ShowArray<T>(this T[,,,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += "[ ";
                    for (int i = 0; i < tab.GetLength(2); i++)
                    {
                        text += "[ ";
                        for (int j = 0; j < tab.GetLength(3); j++)
                        {
                            text += tab[l, c, i, j].ToString() + ", ";
                        }
                        text = text.Remove(text.Length - 2, 2);
                        text += " ], ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 3, 3);
            text += "]";
            Debug.WriteLine(text);
        }
        public static void ShowArray<T>(this T[,,,,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += "[ ";
                    for (int i = 0; i < tab.GetLength(2); i++)
                    {
                        text += "[ ";
                        for (int j = 0; j < tab.GetLength(3); j++)
                        {
                            text += "[ ";
                            for (int k = 0; k < tab.GetLength(4); k++)
                            {
                                text += tab[l, c, i, j, k].ToString() + ", ";
                            }
                            text = text.Remove(text.Length - 2, 2);
                            text += " ], ";
                        }
                        text = text.Remove(text.Length - 2, 2);
                        text += " ], ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 3, 3);
            text += "]";
            Debug.WriteLine(text);
        }
        #endregion

        #region Normalise Array

        /// <summary>
        /// Normalise tout les éléments de l'array pour obtenir des valeur entre 0f et 1f, ainse le min de array sera 0f, et le max 1f.
        /// </summary>
        /// <param name="array">The array to normalised</param>
        public static void Normalise(this float[] array)
        {
            float min = float.MaxValue, max = float.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
                if (array[i] > max)
                    max = array[i];
            }
            float maxMinusMin = max - min;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (array[i] - min) / maxMinusMin;
            }
        }
        /// <summary>
        /// Change array like the sum of each element make 1f
        /// </summary>
        /// <param name="array">the array to change, each element must to be positive</param>
        public static void GetProbabilityArray(this float[] array)
        {
            float sum = 0f;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < 0f)
                {
                    Debug.WriteLine("Array[" + i + "] must to be positive : " + array[i]);
                    return;
                }
                sum += array[i];
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] /= sum;
            }
        }
        #endregion

        #region Shuffle

        public static void Shuffle<T>(this List<T> list)
        {
            for(int i = list.Count; i >= 0; i--)
            {
                int j = Random.Rand(0, i);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
        /// <summary>
        /// Shuffle a little bit the list, reproduce approximately the real life
        /// </summary>
        /// <param name="percentage">The percentage to shuffle between 0 and 1</param>
        public static void ShufflePartialy<T>(this List<T> list, in float percentage)
        {
            int nbPermut = (int)(list.Count * percentage);
            for (int i = 0; i < nbPermut; i++)
            {
                int randIndex1 = Random.RandExclude(0, list.Count);
                int randIndex2 = Random.RandExclude(0, list.Count);
                T temp = list[randIndex1];
                list[randIndex1] = list[randIndex2];
                list[randIndex2] = temp;
            }
        }

        #endregion

        public static List<T> Clone<T>(this List<T> lst) => new List<T>(lst);

        public static void Show<T>(this List<T> lst)
        {
            Debug.Write("[");
            for (int i = 0; i < lst.Count - 1; i++)
            {
                Debug.WriteLine(lst[i].ToString() + ", ");
            }
            Debug.Write(lst[lst.Count - 1].ToString() + "]");
            Debug.WriteLine("");
        }

        private static string[] letter = new string[36] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        private static string Troncate(string mot)
        {
            string newMot = mot;
            for (int i = 0; i < mot.Length; i++)
            {
                string s = mot.Substring(i, 1);
                if (s == "," || s == ".")
                {
                    newMot = newMot.Remove(i, mot.Length - i);
                    break;
                }
            }
            return newMot;
        }

        private static string RemoveSpace(string s)
        {
            string res = s;
            while (res[0] == ' ')
            {
                res = res.Remove(0, 1);
            }
            return res;
        }

        public static int ToInt(this string number)
        {
            int nb = 0;
            number = RemoveSpace(number);
            number = Troncate(number);
            for (int i = number.Length - 1; i >= 0; i--)
            {
                string carac = number.Substring(i, 1);
                for (int j = 26; j <= 35; j++)
                {
                    if (carac == letter[j])
                    {
                        int n = j - 26;
                        nb += n * (int)(float)Math.Pow(10, number.Length - 1 - i);
                        break;
                    }
                }
            }
            if (number.Substring(0, 1) == "-")
                nb *= -1;

            return nb;
        }

        public static float ToFloat(this string number)
        {
            float result = 0;
            string partieEntiere = number;
            string partieDecimal = "";

            int indexComa = 0;
            for (int i = 0; i < number.Length; i++)
            {
                string s = number.Substring(i, 1);
                if (s == "," || s == ".")
                {
                    partieDecimal = partieEntiere.Substring(i + 1, partieEntiere.Length - (i + 1));
                    partieEntiere = partieEntiere.Remove(i, partieEntiere.Length - i);
                    indexComa = i;
                    break;
                }
            }
            //part entière
            result = partieEntiere.ToInt();
            //part decimal
            for (int i = 0; i < partieDecimal.Length; i++)
            {
                string carac = partieDecimal.Substring(i, 1);
                for (int j = 26; j <= 35; j++)
                {
                    if (carac == letter[j])
                    {
                        int n = j - 26; //n € {0,1,2,3,4,5,6,7,8,9}
                        result += n * (float)Math.Pow(10, -(i + 1));
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">la chaine de char ou l'on extrait les données</param>
        /// <param name="startPattern">Le pattern de début d'extraction de données</param>
        /// <param name="endPattern">Le pattern de fin d'extraction de données</param>
        /// <returns>L'ensemble des string entre le début de pattern et le fin de pattern</returns>
        public static List<string> GetSubstringsBetweenPatterns(this string s, string startPattern, string endPattern)
        {
            List<string> res = new List<string>();

            char c;
            bool detectModule = false;
            int indexBeg = 0, indexEnd = 0;
            StringBuilder currentModule = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];
                if (detectModule)
                {
                    if (c == startPattern[indexBeg])
                    {
                        if (indexBeg >= startPattern.Length - 1)
                        {
                            currentModule.Clear();
                            indexBeg = 0;
                        }
                        else
                        {
                            indexBeg++;
                        }
                    }
                    else
                    {
                        indexBeg = 0;
                    }
                    if (c == endPattern[indexEnd])
                    {
                        if (indexEnd >= endPattern.Length - 1)
                        {
                            string tmp = currentModule.ToString();
                            tmp = tmp.Remove(tmp.Length - endPattern.Length + 1, endPattern.Length - 1);
                            res.Add(tmp);
                            currentModule.Clear();
                            detectModule = false;
                            indexEnd = 0;
                            continue;
                        }
                        else
                        {
                            indexEnd++;
                        }
                    }
                    else
                    {
                        indexEnd = 0;
                    }

                    currentModule.Append(c);
                }
                else
                {
                    if (c == startPattern[indexBeg])
                    {
                        if (indexBeg >= startPattern.Length - 1)
                        {
                            detectModule = true;
                            indexBeg = 0;
                        }
                        else
                        {
                            indexBeg++;
                        }
                    }
                    else
                    {
                        indexBeg = 0;
                    }
                }
            }
            return res;
        }

        #region SumList
        /// <summary>
        /// Retourne lst1 union lst2
        /// </summary>
        /// <param name="lst1">La première liste</param>
        /// <param name="lst2">La seconde liste</param>
        /// <param name="doublon">SI on autorise ou pas les doublons</param>
        /// <returns></returns>        
        public static List<T> SumList<T>(in List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
        {
            List<T> result = new List<T>();
            foreach (T nb in lst1)
            {
                if (doublon || !result.Contains(nb))
                    result.Add(nb);
            }
            foreach (T nb in lst2)
            {
                if (doublon || !result.Contains(nb))
                    result.Add(nb);
            }
            return result;
        }
        public static List<T> SumList<T>(this List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
        {
            return SumList(lst1, lst2);
        }
        public static void Add<T>(this List<T> lst1, in List<T> lstToAdd, bool doublon = false)//pas de doublon par defaut
        {
            if (doublon)
            {
                foreach (T element in lstToAdd)
                {
                    lst1.Add(element);
                }
            }
            else
            {
                foreach (T element in lstToAdd)
                {
                    if (lst1.Contains(element))
                    {
                        continue;
                    }
                    lst1.Add(element);
                }
            }

        }
        #endregion

        #region ConvertStingToArray

        public static object ConvertStingToArray(string tab, out int dimension)
        {
            tab = tab.Replace(" ", "");//on enlève tout les espaces
            int dim = 0;//on calcule la dim
            int compteurDim = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                    compteurDim--;
                dim = (int)(float)Math.Max(dim, compteurDim);
            }
            dimension = dim;
            switch (dim)
            {
                case 1:
                    return ConvertStringToArray1(tab);
                case 2:
                    return ConvertStringToArray2(tab);
                case 3:
                    return ConvertStringToArray3(tab);
                case 4:
                    return ConvertStringToArray4(tab);
                case 5:
                    return ConvertStringToArray5(tab);
                default:
                    throw new Exception("To many dim in " + tab + " max dim is 5");
            }
        }
        private static object ConvertStringToArray1(string tab)
        {
            List<string> value = new List<string>();
            string val = "";
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "," || tab[i].ToString() == "]" || tab[i].ToString() == "[")
                {
                    if (val != "")
                        value.Add(val);
                    val = "";
                }
                else
                {
                    val += tab[i].ToString();
                }
            }

            int[] result = new int[value.Count];
            for (int i = 0; i < value.Count; i++)
            {
                result[i] = value[i].ToInt();
            }
            return result;
        }
        private static object ConvertStringToArray2(string tab)
        {
            //"[[1,2,3,4],[4,5,6]]" va retourné [[1, 2, 3, 4], [4, 5, 6, -1]]
            int nbline = -1, nbCol = 0;
            int compteurDim = -1;
            int compteurCol = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                {
                    compteurDim--;
                    nbline++;
                }
                if (compteurDim == 1)
                {
                    if (tab[i].ToString() == ",")
                    {
                        compteurCol++;
                        nbCol = (int)(float)Math.Max(nbCol, compteurCol + 1);
                    }
                }
                else
                {
                    compteurCol = 0;
                }
            }
            int[,] result = new int[nbline, nbCol];//on crée et initialise le tab;
            for (int l = 0; l < result.GetLength(0); l++)
            {
                for (int c = 0; c < result.GetLength(1); c++)
                {
                    result[l, c] = -1;
                }
            }
            //on remplit le resulat
            compteurDim = -1;
            compteurCol = 0;
            int compteurLine = -2;
            string value = "";
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() != "[" && tab[i].ToString() != "]" && tab[i].ToString() != ",")
                {
                    value += tab[i].ToString();
                }
                if (tab[i].ToString() == "[")
                {
                    compteurDim++;
                    compteurLine++;
                    compteurCol = 0;
                }
                else
                {
                    if (tab[i].ToString() == "]")
                    {
                        compteurDim--;
                        if (value != "")
                        {
                            result[compteurLine, compteurCol] = value.ToInt();
                            value = "";
                        }
                    }
                    else
                    {
                        if (tab[i].ToString() == ",")
                        {
                            if (value != "")
                            {
                                result[compteurLine, compteurCol] = value.ToInt();
                                value = "";
                                compteurCol++;
                            }
                        }
                    }
                }
            }

            return result;
        }
        private static object ConvertStringToArray3(string tab)
        {
            return null;
        }
        private static object ConvertStringToArray4(string tab)
        {
            int nbDim0 = 1, nbDim1 = 1, nbDim2 = 1, nbDim3 = 1;
            int compteurDim0 = 0, compteurDim1 = 0, compteurDim2 = 0, compteurDim3 = 0;
            int compteurDim = -1;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                    compteurDim--;
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                            nbDim0 = (int)(float)Math.Max(nbDim0, compteurDim0 + 1);
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                            nbDim1 = (int)(float)Math.Max(nbDim1, compteurDim1 + 1);
                        }
                        break;
                    case 2:
                        compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                            nbDim2 = (int)(float)Math.Max(nbDim2, compteurDim2 + 1);
                        }
                        break;
                    case 3:
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim3++;
                            nbDim3 = (int)(float)Math.Max(nbDim3, compteurDim3 + 1);
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }

            int[,,,] result = new int[nbDim0, nbDim1, nbDim2, nbDim3];//on crée et initialise le tab;
            for (int a = 0; a < result.GetLength(0); a++)
            {
                for (int b = 0; b < result.GetLength(1); b++)
                {
                    for (int c = 0; c < result.GetLength(2); c++)
                    {
                        for (int d = 0; d < result.GetLength(3); d++)
                        {
                            result[a, b, c, d] = -1;
                        }
                    }
                }
            }
            compteurDim0 = compteurDim1 = compteurDim2 = compteurDim3 = 0;
            compteurDim = -1;
            string value = "";
            string text;

            for (int i = 0; i < tab.Length; i++)
            {
                text = tab[i].ToString();
                if (tab[i].ToString() != "[" && tab[i].ToString() != "]" && tab[i].ToString() != ",")
                    value += tab[i].ToString();
                if (tab[i].ToString() == "[")
                {
                    compteurDim++;
                    //compteurDim0++;
                }
                if (tab[i].ToString() == "]")
                {
                    compteurDim--;
                    if (value != "")
                    {
                        result[compteurDim0, compteurDim1, compteurDim2, compteurDim3] = value.ToInt();
                        value = "";
                    }
                }
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                        }
                        break;
                    case 2:
                        compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                        }
                        break;
                    case 3:
                        if (tab[i].ToString() == ",")
                        {
                            result[compteurDim0, compteurDim1, compteurDim2, compteurDim3] = value.ToInt();
                            value = "";
                            compteurDim3++;
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }
            return result;
        }
        private static object ConvertStringToArray5(string tab)
        {
            int nbDim0 = 1, nbDim1 = 1, nbDim2 = 1, nbDim3 = 1, nbDim4 = 1;
            int compteurDim0 = 0, compteurDim1 = 0, compteurDim2 = 0, compteurDim3 = 0, compteurDim4 = 0;
            int compteurDim = -1;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                    compteurDim--;
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                            nbDim0 = (int)(float)Math.Max(nbDim0, compteurDim0 + 1);
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                            nbDim1 = (int)(float)Math.Max(nbDim1, compteurDim1 + 1);
                        }
                        break;
                    case 2:
                        compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                            nbDim2 = (int)(float)Math.Max(nbDim2, compteurDim2 + 1);
                        }
                        break;
                    case 3:
                        compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim3++;
                            nbDim3 = (int)(float)Math.Max(nbDim3, compteurDim3 + 1);
                        }
                        break;
                    case 4:
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim4++;
                            nbDim4 = (int)(float)Math.Max(nbDim4, compteurDim4 + 1);
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }

            int[,,,,] result = new int[nbDim0, nbDim1, nbDim2, nbDim3, nbDim4];//on crée et initialise le tab;
            for (int a = 0; a < result.GetLength(0); a++)
            {
                for (int b = 0; b < result.GetLength(1); b++)
                {
                    for (int c = 0; c < result.GetLength(2); c++)
                    {
                        for (int d = 0; d < result.GetLength(3); d++)
                        {
                            for (int e = 0; e < result.GetLength(4); e++)
                            {
                                result[a, b, c, d, e] = -1;
                            }
                        }
                    }
                }
            }
            compteurDim0 = compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
            compteurDim = -1;
            string value = "";

            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() != "[" && tab[i].ToString() != "]" && tab[i].ToString() != ",")
                    value += tab[i].ToString();
                if (tab[i].ToString() == "[")
                {
                    compteurDim++;
                    //compteurDim0++;
                }
                if (tab[i].ToString() == "]")
                {
                    compteurDim--;
                    if (value != "")
                    {
                        result[compteurDim0, compteurDim1, compteurDim2, compteurDim3, compteurDim4] = value.ToInt();
                        value = "";
                    }
                }
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                        }
                        break;
                    case 2:
                        compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                        }
                        break;
                    case 3:
                        compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim3++;
                        }
                        break;
                    case 4:
                        if (tab[i].ToString() == ",")
                        {
                            result[compteurDim0, compteurDim1, compteurDim2, compteurDim3, compteurDim4] = value.ToInt();
                            value = "";
                            compteurDim4++;
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }
            return result;
        }
        #endregion

        #region ConvertArrayToString
        public static string ConvertArrayToString<T>(in object array, in int dimension)
        {
            return ConvertArrayToString<T>(array, dimension, ToString);
        }
        private static string ToString<T>(T obj) => obj.ToString();

        public static string ConvertArrayToString<T>(in object array, in int dimension, SerialyseFunction<T> convertFunction)
        {
            switch (dimension)
            {
                case 1:
                    return ConvertArrayToString1((T[])array, convertFunction);
                case 2:
                    return ConvertArrayToString2((T[,])array, convertFunction);
                case 3:
                    return ConvertArrayToString3((T[,,])array, convertFunction);
                case 4:
                    return ConvertArrayToString4((T[,,,])array, convertFunction);
                case 5:
                    return ConvertArrayToString5((T[,,,,])array, convertFunction);
                default:
                    throw new Exception("Too many dimension in ConvertArrayToString, maximun 5");
            }
        }

        private static string ConvertArrayToString1<T>(in T[] array, SerialyseFunction<T> convertFunction)
        {
            string result = "[";
            for (int i = 0; i < array.Length; i++)
            {
                result += convertFunction(array[i]) + ",";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString2<T>(in T[,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "[";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += convertFunction(array[l, c]) + ",";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString3<T>(in T[,,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += "[";
                    for (int i = 0; i < array.GetLength(2); i++)
                    {
                        result += convertFunction(array[l, c, i]) + ",";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString4<T>(in T[,,,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += "[";
                    for (int i = 0; i < array.GetLength(2); i++)
                    {
                        result += "[";
                        for (int j = 0; j < array.GetLength(3); j++)
                        {
                            result += convertFunction(array[l, c, i, j]) + ",";
                        }
                        result = result.Remove(result.Length - 1, 1) + "]";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString5<T>(in T[,,,,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += "[";
                    for (int i = 0; i < array.GetLength(2); i++)
                    {
                        result += "[";
                        for (int j = 0; j < array.GetLength(3); j++)
                        {
                            result += "[";
                            for (int k = 0; k < array.GetLength(4); k++)
                            {
                                result += convertFunction(array[l, c, i, j, k]) + ",";
                            }
                            result = result.Remove(result.Length - 1, 1) + "]";
                        }
                        result = result.Remove(result.Length - 1, 1) + "]";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        #endregion

        public static float NearestFromzero(in float a, in float b) => (float)Math.Abs(a) <= (float)Math.Abs(b) ? a : b;
        public static float FarestFromzero(in float a, in float b) => (float)Math.Abs(a) >= (float)Math.Abs(b) ? a : b;

        #region Integrate

        public static float[] xi = new float[5] { 0f, 0.5384693f, -0.5384693f, 0.90617985f, -0.90617985f };
        public static float[] wi = new float[5] { 0.5688889f, 0.47862867f, 0.47862867f, 0.23692688f, 0.23692688f };

        /// <summary>
        /// Rerturn the integral between a and b of f(x)dx
        /// </summary>
        /// <param name="function">La function à intégré</param>
        /// <param name="a">le début de l'intégrale</param>
        /// <param name="b">la fin de l'intégrale</param>
        /// <param name="stepPerUnit">le nombre de point évalué par unité</param>
        /// <returns>The integral between a and b of f(x)dx</returns>
        public static float Integrate(Function f, in float a, in float b, int accuracy = 15)
        {
            if (a == b)
                return 0f;
            if (a > b)
                return -Integrate(f, b, a, accuracy);
            if (accuracy <= 1)
                accuracy = 1;

            float res = 0f;
            float h = (b - a) / accuracy;
            float hO2 = h * 0.5f;
            float mid = a + hO2;

            for (float i = a; i <= (b - hO2); i += h)
            {
                //calcule lint entre i et i + h
                for (int j = 0; j < 5; j++)
                {
                    res += wi[j] * f(hO2 * xi[j] + mid);
                }
                mid += h;
            }
            return hO2 * res;
        }

        #endregion
    }
}