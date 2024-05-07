using Autodesk.Connectivity.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs_vault_bridge_console.Util
{
	internal class Tree<T>
	{
		Node<T> root;
		Dictionary<long, int> partsTotalQuatity;
		int depth;

		//	TODO : Change code to initialize root with constructor
		public bool AddRoot(long rootId, Node<T> rootItem) {
			if (root == null) { 	
				this.root = rootItem;
				this.root.nodeID = rootId;
				return true;
			}
			return false;
		}

		//	TODO : Improve algorithm rather than use DFS
		//	Add Item to list.
		//	If parent id is not equal to root than find 
		public void Add(long parentId, Node<T> item) {
			root.AddChild(parentId, item);
		}
	}
	internal class Node<T> {
		public long nodeID;
		public Node<T> Parent { get; set; }
		public List< Node<T> > Child { get; set; }
		public T Value{ get; set; }
		public Node(T toAdd, ItemAssoc itemAssoc) {
			this.Value = toAdd;
			nodeID = itemAssoc.CldItemID;

		}
		// 자식이 없다면 리스트를 생성한뒤 추가.
		// 자식이 존재 한다면 단순히 리스트에 자식 추가.
		public bool AddChild(long parentId, Node<T> toAdd) {
			if (this.Child == null)
			{
				this.Child = new List<Node<T>>
				{
					toAdd
				};
				return true;
			}
			else
			{
				this.Child.Add(toAdd);
				return true;
			}
		}
		// 부모가 없으면 부모를 추가.
		// 부모가 있으면 추가 하지 않고 false를 반환.
		public bool AddParent(Node<T> toAdd) {
			if (toAdd == null) {
				this.Parent = null;
			}
			if (this.Parent == null)
			{
				this.Parent = toAdd;
				return true;
			}
			return false;
		}
	}
	internal class CustomItem {
		public long id;
		public string title;
		public UnitType unitType;
		public ItemCategory itemCategory;
		public ItemType itemType;
		public string code;
		public string specification;
		public bool parentsCheck;
		public int safetyStock;
		public string note;
		//MES 에서 사용하는 변수 매핑 편의 위해 넣어둠
		public int quantity;
		public double inspectionRate;
		public int totalInventoryQuatity;

		public CustomItem(long id, string title) {
			this.id = id;
			this.title = title;
		}

		public CustomItem(long id, string title, UnitType unitType, ItemCategory itemCategory, ItemType itemType, string code, string specification, bool parentCheck, int safetyStock, string note, int queatity) { 
			this.specification = specification;
			this.id = id;
			this.title = title;	
			this.unitType = unitType;	
			this.itemCategory = itemCategory;
			this.itemType = itemType;
			this.code = code;
			this.parentsCheck = parentCheck;
			this.safetyStock = safetyStock; 
			this.note = note;
			this.quantity = queatity;
		}
	
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
