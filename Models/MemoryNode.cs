using System;
using System.Collections.Generic;

namespace MemoryVisualizer.Models
{
    /// <summary>
    /// Represents a node in the memory hierarchy.
    /// This class is used for JSON deserialization and storing node data.
    /// </summary>
    public class MemoryNode
    {
        /// <summary>
        /// Unique identifier for the node
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Label for the node
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Type of the node (e.g., "Company", "Division", "Employee", "Product", "Revenue")
        /// Used for color-coding in the visualization
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Additional properties for the node
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Collection of child nodes in the hierarchy
        /// </summary>
        public List<MemoryNode> Children { get; set; }

        /// <summary>
        /// Parent node in the hierarchy
        /// </summary>
        public MemoryNode? Parent { get; set; }

        /// <summary>
        /// ToolTip for the node
        /// </summary>
        public string? ToolTip { get; set; }

        /// <summary>
        /// Initializes a new instance of the MemoryNode class.
        /// </summary>
        public MemoryNode()
        {
            Properties = new Dictionary<string, object>();
            Children = new List<MemoryNode>();
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Adds a child node to the current node.
        /// </summary>
        /// <param name="child">The child node to add.</param>
        public void AddChild(MemoryNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }
}
