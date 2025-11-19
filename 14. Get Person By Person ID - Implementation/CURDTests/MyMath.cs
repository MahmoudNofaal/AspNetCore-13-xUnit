using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CURDTests;

public class MyMath
{
   public int Add(int a, int b)
   {
      int c = a + b;

      return c;

      //// We deliberately introduce a bug here for demonstration purposes
      //return 0;
   }

}
