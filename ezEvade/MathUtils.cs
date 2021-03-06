﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;

namespace ezEvade
{
    class MathUtils
    {

        public static bool CheckLineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            Tuple<float, float> ret = LineToLineIntersection(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y);

            var t1 = ret.Item1;
            var t2 = ret.Item2;

            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Vector2 RotateVector(Vector2 start, Vector2 end, float angle)
        {
            angle = angle * ((float)(Math.PI / 180));
            Vector2 ret = end;
            ret.X = ((float)Math.Cos(angle) * (end.X - start.X) -
                (float)Math.Sin(angle) * (end.Y - start.Y) + start.X);
            ret.Y = ((float)Math.Sin(angle) * (end.X - start.X) +
                (float)Math.Cos(angle) * (end.Y - start.Y) + start.Y);
            return ret;
        }

        public static Tuple<float, float> LineToLineIntersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            var d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

            if (d == 0)
            {
                return Tuple.Create(float.MaxValue, float.MaxValue); //lines are parallel or coincidental
            }
            else
            {
                return Tuple.Create(((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / d,
                    ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / d);
            }
        }

        /*
         * 
         * //from leaguesharp.commons
                var spellPos = spell.GetCurrentSpellPosition(true);
                var sol = Geometry.VectorMovementCollision(spellPos, spell.endPos, spell.info.projectileSpeed, heroPos, myHero.MoveSpeed);

                var startTime = 0f;
                var endTime = spellPos.Distance(spell.endPos) / spell.info.projectileSpeed;

                var time = (float) sol[0];
                var pos = (Vector2) sol[1];

                if (pos.IsValid() && time >= startTime && time <= startTime + endTime)
                {
                    return true;
                }
         * 
         */


        public static float VectorMovementCollisionEx(Vector2 targetPos, Vector2 targetDir, float targetSpeed, Vector2 sourcePos, float projSpeed, out bool collision, float extraDelay = 0, float extraDist = 0)
        {
            Vector2 velocity = targetDir * targetSpeed;
            targetPos = targetPos - velocity * (extraDelay / 1000);

            float velocityX = velocity.X;
            float velocityY = velocity.Y;

            Vector2 relStart = targetPos - sourcePos;

            float relStartX = relStart.X;
            float relStartY = relStart.Y;

            float a = velocityX * velocityX + velocityY * velocityY - projSpeed * projSpeed;
            float b = 2 * velocityX * relStartX + 2 * velocityY * relStartY;
            float c = Math.Max(0, relStartX * relStartX + relStartY * relStartY + extraDist * extraDist);
            
            float disc = b * b - 4 * a * c;

            if (disc >= 0)
            {
                float t1 = -(b + (float)Math.Sqrt(disc)) / (2 * a);
                float t2 = -(b - (float)Math.Sqrt(disc)) / (2 * a);

                collision = true;

                if (t1 > 0 && t2 > 0)
                {
                    return (t1 > t2) ? t2 : t1;

                }
                else if (t1 > 0)
                    return t1;
                else if (t2 > 0)
                    return t2;
            }

            collision = false;

            return 0;
        }

        public static bool PointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            var dotProduct = Vector2.Dot((end - start), (point - start));
            if (dotProduct < 0)
                return false;

            var lengthSquared = Vector2.DistanceSquared(start, end);
            if (dotProduct > lengthSquared)
                return false;

            return true;
        }

        //https://code.google.com/p/xna-circle-collision-detection/downloads/detail?name=Circle%20Collision%20Example.zip&can=2&q=

        public static float GetCollisionTime(Vector2 Pa, Vector2 Pb, Vector2 Va, Vector2 Vb, float Ra, float Rb, out bool collision)
        {
            Vector2 Pab = Pa - Pb;
            Vector2 Vab = Va - Vb;
            float a = Vector2.Dot(Vab, Vab);
            float b = 2 * Vector2.Dot(Pab, Vab);
            float c = Vector2.Dot(Pab, Pab) - (Ra + Rb) * (Ra + Rb);

            float discriminant = b * b - 4 * a * c;

            float t;
            if (discriminant < 0)
            {
                t = -b / (2 * a);
                collision = false;
            }
            else
            {
                float t0 = (-b + (float)Math.Sqrt(discriminant)) / (2 * a);
                float t1 = (-b - (float)Math.Sqrt(discriminant)) / (2 * a);
                t = Math.Min(t0, t1);

                if (t < 0)
                    collision = false;
                else
                    collision = true;
            }

            if (t < 0)
                t = 0;

            return t;
        }        

        //http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        // Find the points of intersection.
        public static int FindLineCircleIntersections(
            float cx, float cy, float radius,
            Vector2 point1, Vector2 point2,
            out Vector2 intersection1, out Vector2 intersection2)
        {
            float dx, dy, A, B, C, det, t;

            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) +
                (point1.Y - cy) * (point1.Y - cy) -
                radius * radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);
                intersection1 =
                    new Vector2(point1.X + t * dx, point1.Y + t * dy);               
                intersection2 = new Vector2(float.NaN, float.NaN);

                return 1;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 =
                    new Vector2(point1.X + t * dx, point1.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 =
                    new Vector2(point1.X + t * dx, point1.Y + t * dy);          

                return 2;
            }
        }
    }
}
