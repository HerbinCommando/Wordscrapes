using System;
using System.Collections.Generic;

public class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; private set; }
    public bool IsEndOfWord { get; set; }

    public TrieNode()
    {
        Children = new Dictionary<char, TrieNode>();
        IsEndOfWord = false;
    }
}

public class Trie
{
    private TrieNode root;

    public Trie()
    {
        root = new TrieNode();
    }

    public void Insert(string word)
    {
        TrieNode current = root;
        foreach (char c in word)
        {
            if (!current.Children.ContainsKey(c))
                current.Children[c] = new TrieNode();

            current = current.Children[c];
        }
        current.IsEndOfWord = true;
    }

    public bool Search(string word)
    {
        TrieNode node = SearchNode(word);
        return node != null && node.IsEndOfWord;
    }

    private TrieNode SearchNode(string word)
    {
        TrieNode current = root;
        foreach (char c in word)
        {
            if (current.Children.ContainsKey(c))
                current = current.Children[c];
            else
                return null;
        }
        return current;
    }

    public List<string> GeneratePermutations(string word)
    {
        List<string> permutations = new List<string>();
        GeneratePermutationsRecursive(word, root, "", permutations);
        return permutations;
    }

    private void GeneratePermutationsRecursive(string word, TrieNode node, string prefix, List<string> permutations)
    {
        if (node.IsEndOfWord)
        {
            permutations.Add(prefix);
        }

        foreach (char c in node.Children.Keys)
        {
            string nextPrefix = prefix + c;
            GeneratePermutationsRecursive(word, node.Children[c], nextPrefix, permutations);
        }
    }
}

public class PermutationGenerator
{
    private Trie trie;

    public PermutationGenerator()
    {
        trie = new Trie();
    }

    public List<string> GeneratePermutations(string word)
    {
        GeneratePermutations("", word, trie);
        return trie.GeneratePermutations(word);
    }

    private void GeneratePermutations(string prefix, string word, Trie trie)
    {
        trie.Insert(prefix);
        for (int i = 0; i < word.Length; i++)
        {
            GeneratePermutations(prefix + word[i], word.Substring(0, i) + word.Substring(i + 1), trie);
        }
    }
}