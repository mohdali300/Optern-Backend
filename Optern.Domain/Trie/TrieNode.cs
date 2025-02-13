namespace Optern.Domain.Trie
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; set; } = new Dictionary<char, TrieNode>();
        public bool IsEndOfWord { get; set; } = false;
    }
}
