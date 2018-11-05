using System;

namespace System . Windows . Controls
{
	public static class MyExt
	{
		public static void PerformClick ( this Button btn )
		{
			btn . RaiseEvent ( new RoutedEventArgs ( routedEvent: Primitives . ButtonBase . ClickEvent ) );
		}

		public static void PerformClick ( this CheckBox cb )
		{
			cb . RaiseEvent ( new RoutedEventArgs ( Primitives . ButtonBase . ClickEvent ) );
		}
	}
}
