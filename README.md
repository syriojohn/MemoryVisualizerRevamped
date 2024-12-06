# Memory Visualizer Revamped

A powerful WPF-based application for visualizing hierarchical memory structures using Microsoft Automatic Graph Layout (MSAGL).

## Features

### Core Visualization
- Interactive graph visualization using Microsoft MSAGL
- Multiple layout algorithms (Sugiyama, MDS, Ranking)
- Color-coded nodes based on entity types
- Tooltips with detailed information

### Data Structure Support
- Hierarchical node representation
- Support for various entity types:
  - Companies
  - Divisions
  - Employees
  - Products
  - Revenue information
- Rich node properties including labels, types, and custom attributes

### User Interface
- Clean, modern WPF interface
- Interactive graph viewer
- Layout algorithm selector
- Refresh and Export capabilities

## Technical Details

### Built With
- .NET 8.0
- Windows Presentation Foundation (WPF)
- Microsoft Automatic Graph Layout (MSAGL)
- Windows Forms Integration (for MSAGL viewer)

### Node Types and Colors
- Company: Light Blue
- Division: Light Green
- Employee: Light Yellow
- Product: Light Pink
- Revenue: Light Gray

## Development Roadmap

### Current Features
- Basic graph visualization
- Hierarchical data representation
- Sample data generation
- Multiple layout algorithms
- Color-coded nodes
- Tooltips

### Planned Features
- Export functionality
- External data source support
- Enhanced layout options
- Additional visualization styles
- Performance optimizations

## Getting Started

### Prerequisites
- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows 10/11

### Building
1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Build and run

## Sample Data
The application comes with a built-in sample dataset representing a company structure:
- Root company (TechSales Corp)
  - Hardware Division
    - Multiple employees
    - Hardware products
  - Software Division
    - Multiple employees
    - Software products

Each entity includes realistic metadata such as revenue figures, job titles, and relationships.
