% Part of the MetaUML manual.
% Copyright (C) 2005 Ovidiu Gheorghies
%
% This program is free software; you can redistribute it and/or
% modify it under the terms of the GNU General Public License
% as published by the Free Software Foundation; either version 2
% of the License, or (at your option) any later version.
%
% This program is distributed in the hope that it will be useful,
% but WITHOUT ANY WARRANTY; without even the implied warranty of
% MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
% GNU General Public License for more details.
%
% You should have received a copy of the GNU General Public License
% along with this program; if not, write to the Free Software
% Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

input metauml;

beginfig(1);
    Class.BAGReader("BAG Reader")()();
    BAGReader.info.iAttributeStack.top := 0;
    BAGReader.info.iAttributeStack.bottom := 0;
    BAGReader.info.iMethodStack.top := 0;
    BAGReader.info.iMethodStack.bottom := 0;
    Class.OSMReader("OSM Reader")()();
    OSMReader.info.iAttributeStack.top := 0;
    OSMReader.info.iAttributeStack.bottom := 0;
    OSMReader.info.iMethodStack.top := 0;
    OSMReader.info.iMethodStack.bottom := 0;
    Package.Preprocessor("Data Preprocessor")(BAGReader, OSMReader);
    Preprocessor.info.forceEmptyContent := 1;

    Class.Building("Building")()();
    Building.info.iAttributeStack.top := 0;
    Building.info.iAttributeStack.bottom := 0;
    Building.info.iMethodStack.top := 0;
    Building.info.iMethodStack.bottom := 0;
    Class.OSMElement("OSM Element")()();
    OSMElement.info.iAttributeStack.top := 0;
    OSMElement.info.iAttributeStack.bottom := 0;
    OSMElement.info.iMethodStack.top := 0;
    OSMElement.info.iMethodStack.bottom := 0;
    Class.Tree("Tree")()();
    Tree.info.iAttributeStack.top := 0;
    Tree.info.iAttributeStack.bottom := 0;
    Tree.info.iMethodStack.top := 0;
    Tree.info.iMethodStack.bottom := 0;
    Class.Node("Node")()();
    Node.info.iAttributeStack.top := 0;
    Node.info.iAttributeStack.bottom := 0;
    Node.info.iMethodStack.top := 0;
    Node.info.iMethodStack.bottom := 0;
    Package.Treebuilder("Treebuilder and Model Contstructor")(Building, OSMElement, Tree, Node);
    Treebuilder.info.forceEmptyContent := 1;

    Class.VertexBuffer("Vertex Buffer")()();
    VertexBuffer.info.iAttributeStack.top := 0;
    VertexBuffer.info.iAttributeStack.bottom := 0;
    VertexBuffer.info.iMethodStack.top := 0;
    VertexBuffer.info.iMethodStack.bottom := 0;
    Class.NodeManager("Node Manager")()();
    NodeManager.info.iAttributeStack.top := 0;
    NodeManager.info.iAttributeStack.bottom := 0;
    NodeManager.info.iMethodStack.top := 0;
    NodeManager.info.iMethodStack.bottom := 0;
    Class.Visualizer("Visualizer")()();
    Visualizer.info.iAttributeStack.top := 0;
    Visualizer.info.iAttributeStack.bottom := 0;
    Visualizer.info.iMethodStack.top := 0;
    Visualizer.info.iMethodStack.bottom := 0;
    Package.Visualize("Visualize")(VertexBuffer, NodeManager, Visualizer);
    Visualize.info.forceEmptyContent := 1;

    leftToRight(10)(BAGReader, OSMReader);
    topToBottom(20)(Tree, Node);
    topToBottom.right(20)(Node, OSMElement);
    %topToBottom(20)(Node, OSMElement);
    leftToRight(20)(OSMElement, Building);
    leftToRight(30)(Visualizer, NodeManager);
    topToBottom(20)(NodeManager, VertexBuffer);
    topToBottom(60)(Preprocessor, Treebuilder, Visualize);

    drawObject(Preprocessor);
    drawObject(Treebuilder);
    drawObject(Visualize);

    link(association)(Preprocessor.s -- Treebuilder.n + (0,15));
    link(association)(Treebuilder.s -- Visualize.n);
    link(associationUni)(Tree.s -- Node.n);
    link(associationUni)(Node.nw .. Node.w+(-20, 15) .. Node.sw+(-20, -15) .. Node.sw);
    link(associationUni)(Node.s -- Building.n);
    link(associationUni)(Node.s -- OSMElement.n);
    link(associationUni)(Visualizer.e -- NodeManager.w);
    link(associationUni)(NodeManager.s -- VertexBuffer.n);

endfig;

end
