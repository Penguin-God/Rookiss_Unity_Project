// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("4lj9173kwsIuqtgbWSyhK/dyz6eKOLuYire8s5A88jxNt7u7u7+6uVjqZNZdMFrcY7WFOGKW+NniQeY3m+HWmwVBEWQxQWjEVqqwNT0cCwlVQKCnOkpoNTZssB8OKqTx6t/Ixa0kqQ1+g1lw1nQybk9sJ+y0JGnaaQncd9sqnEb4PqeUax3cT+q5GLU4u7W6iji7sLg4u7u6IPcoDaKLge5WLU2h3e3qAS4Z4si/HseFb7W6DCUCiQIjNdY6BE5W2OKmm9UMldaWNm+MIKDbwtgs7I83UFPmYVNdvc1Cyju4sb5XoAc2TqNmCQGiMQlB6H9HnPZNGKYyevsKve25nBMjZjK+tUyNUvBYks6qIJQD6xUsqG27y5pkxnqyffypS7i5u7q7");
        private static int[] order = new int[] { 10,10,9,11,9,9,7,10,9,12,12,12,13,13,14 };
        private static int key = 186;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
