namespace Wintellect.PowerCollections.Tests {
    /// <summary>
    /// An item type used when testing the RedBlackTree.
    /// </summary>
    struct TestItem
    {
        public TestItem(string key)
        {
            this.key = key;
            this.data = 0;
        }
        public TestItem(string key, int data)
        {
            this.key = key;
            this.data = data;
        }
        public string key;
        public int data;

        public override string ToString()
        {
            return string.Format("Key:{0} Data:{1}", key, data);
        }

    }
}