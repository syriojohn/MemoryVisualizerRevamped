using System;
using System.Collections.Generic;

namespace MemoryVisualizer.Models
{
    public class MemoryNode
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }  // Company, Division, Employee, Product, Revenue
        public Dictionary<string, object> Properties { get; set; }
        public List<MemoryNode> Children { get; set; }
        public MemoryNode Parent { get; set; }
        public string ToolTip { get; set; }

        public MemoryNode()
        {
            Properties = new Dictionary<string, object>();
            Children = new List<MemoryNode>();
            Id = Guid.NewGuid().ToString();
        }

        public void AddChild(MemoryNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }
}
