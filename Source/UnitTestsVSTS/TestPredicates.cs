using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wintellect.PowerCollections.Tests {
    static internal class TestPredicates {
        public static bool Even(int x) => (x & 1) == 0;

        public static bool Odd(int x) => (x & 1) == 1;

        public static bool Over10(int x) => x > 10;

        public static bool Under3(int x) => x < 3;

        public static bool Over7(int x) {
            return x > 7;
        }

        public static bool Under10(int x) {
            return x < 10;
        }

        public static int ReverseFirstLetter(string x, string y) {
            if (x[0] < y[0])
                return 1;
            else if (x[0] > y[0])
                return -1;
            else
                return 0;
        }

        public static bool Over5(int x) => x > 5;

        public static bool Over8(int x) => x > 8;

        public static bool Under1(int x) {
            return x < 1;
        }

        public static bool Under4(int x) {
            return x < 4;
        }

        public static bool Over3(int x) {
            return x > 3;
        }

        public static bool ObjectEquals<T>(T x, T y) => Equals(x, y);

        public static bool IsNegative(double x) {
            return x < 0;
        }

        public static bool AbsOver5(double d) {
            return Math.Abs(d) > 5;
        }

        public static bool Under200(double d) {
            return d < 200;
        }

        public static bool IsZero(double d) {
            return d == 0;
        }
    }
}
