using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platinum_rift
{
    public class Node
    {
        private int _id;
        private Node _parent;
        List<Node> _childs;

        public Node(int id) { 
           _id=id;        
        }

        public Node getParent(){
            return _parent;
           }

        public Node[] getChilds(){
            return _childs.ToArray(); ;
        }

        public void setParent(Node newNode){
         _parent=newNode;
        }

        public void setOneChild(Node newChild){
        _childs.Add(newChild);
        }
    }


}
