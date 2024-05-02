using Autodesk.Connectivity.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_vault_bridge_console.Util
{
	internal class Tree<T>
	{
		Node<T> root;
		Dictionary<string, int> partsTotalQuatity;
		int depth;
		
		//TODO : Improve algorithm rather than use DFS
		public Node<T> FindNodeById(long id) {
			return null;
		}
		public void Add(T item, long id) {
			if (root == null) {

			}
		}
	}
	internal class Node<T> { 
		public Node<T> Parent { get; set; }
		public List< Node<T> > Child { get; set; }
		public T Value{ get; set; }
		public Node(T toAdd) {
			this.Value = toAdd;
		}
		public bool AddChild(T toAdd) {
			if (this.Child == null) { 
				this.Child = new List< Node<T> >();
			}
			return false;
		}
		public bool AddParent(T toAdd) {
			return false;
		}
	}
	internal class CustomItem {
		public string title;
		public UnitType unitType;
		public ItemCategory itemCategory;
		public ItemType itemType;
		public string code;
		public string specification;
		public bool parentsCheck;
		//MES 에서 사용하는 변수 매핑 편의 위해 넣어둠
		public int totalInventoryQuatity;
		public int safetyStock;
		public string note;
		//MES 에서 사용하는 변수 매핑 편의 위해 넣어둠
		public double inspectionRate;
		//MES 에서 사용하는 변수 매핑 편의 위해 넣어둠
		public int quantity;
	
	}
	internal enum ItemCategory {
		PURCHASE,
		ASSEMBLY
	}
	internal enum ItemType {
		PNEUMATIC,
		HYDRAULIC,
		BEARING,
		MOTOR,
		PBELT,
		RTPVC,
		ROLLERS,
		MATERIALSA,
		MATERIALSB
	}
	internal enum UnitType {
		EA,
		ROLL,
		BONG,
		T,
		BOX,
		M
	}
}
