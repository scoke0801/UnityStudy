public class Define
{ 
    public class Inven
    {
        public const int INVEN_MAX = 55;
        public const int ITEM_STACK_MAX = 9999;
    }

    public class Input
    {
        public const int LEFT_CLICK = 0;
        public const int RIGHT_CLICK = 1;
    }

    public enum UIEvent
    {
        Click,
        Preseed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }

}
