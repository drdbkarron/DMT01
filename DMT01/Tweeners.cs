using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace DMT01
{
	class Tweeners
	{
		public static bool IsInBetween ( int v1 , int j , int v2 )
		{
			if ( v1 <= j )
			{
				if ( j <= v2 )
				{
					return true;
				}
			}
			if ( v1 >= j )
			{
				if ( j >= v2 )
				{
					return true;
				}
			}

			return false;
		}

	}
}
