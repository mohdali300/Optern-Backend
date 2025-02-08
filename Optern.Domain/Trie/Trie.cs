using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Trie
{
    public class Trie
    {
        private readonly TrieNode _root= new TrieNode();
        public void Insert(string word)
        {
            if (string.IsNullOrEmpty(word))
            { 
                return;
            }

            TrieNode node = _root;
            foreach (char character in word)
            {
                if (!node.Children.ContainsKey(character))
                {
                    node.Children[character] = new TrieNode();
                }
                node = node.Children[character];
            }
            node.IsEndOfWord = true;
        }

        public List<string> GetSuggestionsWords(string searchTerm)
        {
            List<string> suggestions = new List<string>();

            DFS(_root, "", searchTerm,suggestions);

            return suggestions;
        }

        private void DFS(TrieNode node, string currentWord, string searchTerm, List<string> suggestions)
        {
            if (node.IsEndOfWord)
            {
                if (currentWord.Contains(searchTerm)) 
                {
                    suggestions.Add(currentWord);
                }
            }

            foreach (var child in node.Children)
            {
                DFS(child.Value, currentWord + child.Key, searchTerm, suggestions);
            }
        }
    
}
}
