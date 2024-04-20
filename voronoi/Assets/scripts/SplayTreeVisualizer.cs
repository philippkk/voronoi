using UnityEngine;
using TMPro;

public class SplayTreeVisualizer : MonoBehaviour
{
    public SplayTree splayTree; // Reference to your SplayTree class
    public TextMeshProUGUI textMesh;



    public void UpdateTreeDisplay()
    {
        if (splayTree == null)
        {
            Debug.LogError("SplayTree reference is missing!");
            return;
        }

        // Generate the tree format recursively
        string treeFormat = GenerateTreeFormat(splayTree.root, 0);

        // Set the TextMeshPro text
        textMesh.text = treeFormat;
    }

    private string GenerateTreeFormat(Node node, int depth)
    {
        if (node == null)
            return "";

        // Create indentation based on depth
        string indent = new string(' ', 4 * depth);

        // Recursively generate left and right subtrees
        string leftSubtree = GenerateTreeFormat(node.prev, depth + 1);
        string rightSubtree = GenerateTreeFormat(node.next, depth + 1);

        // Combine current node and its subtrees
        string treeFormat = $"{indent}{node.key}\n{leftSubtree}{rightSubtree}";

        return treeFormat;
    }
}
