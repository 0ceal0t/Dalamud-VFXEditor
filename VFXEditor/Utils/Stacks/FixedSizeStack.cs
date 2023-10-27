namespace VfxEditor.Utils.Stacks {
    public class FixedSizeStack<T> {
        private readonly int Capacity;
        private readonly T[] Buffer;
        private int Size;
        private int Head;

        public FixedSizeStack( int capacity ) {
            Capacity = capacity;
            Size = 0;
            Head = 0;
            Buffer = new T[capacity];
        }

        public virtual int Count {
            get { return Size; }
        }

        public virtual T Peek() {
            var item = default( T );

            if( Count > 0 )
                return Buffer[Head % Capacity];

            return item;
        }

        public virtual T Pop() {
            var item = default( T );

            if( Count > 0 ) {
                item = Buffer[Head % Capacity];
                Buffer[Head % Capacity] = default;

                if( Head > 0 ) //Don't decrement the head if it's zero
                    Head--;
                Size--;
            }

            return item;
        }

        public virtual void Push( T item ) {
            if( Count > 0 )
                Head++;

            Buffer[Head % Capacity] = item;

            if( Count < Capacity )
                Size++;
        }

        public virtual void Clear() {
            Size = 0;
            Head = 0;
            for( var i = 0; i < Capacity; i++ ) Buffer[i] = default;
        }
    }
}
