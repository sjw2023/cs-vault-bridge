﻿using Autodesk.Connectivity.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs_vault_bridge_console.Util
{
	// "id": 21,
	//"title": "제품21",
	//"unitType": "T",
	//"itemCategoryType": "ASSEMBLY",
	//"itemType": "PBELT",
	//"code": "F201",
	//"specifications": "1m",
	//"parentsCheck": true,
	//"totalInventoryQuantity": 15,
	//"safetyStock": 0,
	//"note": "비고",
	//"inspectionRate": 0.0,
	//"itemChildren": [],
	//"processTemplates": []
	//"itemProcesses": []
	//"processTemplateSetting": {}
	//internal class Tree<T>
	//{
	//	private Node<T> root; 
	//	public Node<T> Root { get { return this.root; } set { this.root = value; }  }
	//	Dictionary<long, int> partsTotalQuatity;
	//	int depth;

	//	public Tree(){
	//	}

	//	public Tree( Node<T> root, int depth=0 ) { 
	//		this.root = root;
	//		this.root.nodeID = root.nodeID;
	//		this.depth = depth;
	//	}

	//	//	TODO : Change code to initialize root with constructor
	//	public bool AddRoot(long rootId, Node<T> rootItem) {
	//		if (root == null) { 	
	//			this.root = rootItem;
	//			this.root.nodeID = rootId;
	//			return true;
	//		}
	//		return false;
	//	}

	//	//	TODO : Improve algorithm rather than use DFS
	//	//	Add Item to list.
	//	//	If parent id is not equal to root than find 
	//	public void Add( Node<T> parent, Node<T> item) {
	//		root.AddChild( parent, item);
	//	}

	//	//kkjjpublic Node<T> FindNodeById() {
	//	//	Node<T> toRet = null;
	//	//	for
	//	//	return root;
	//	//}
	//	//public bool AddChildToNode(Node<T> toAdd, long id) { 

	//	//}
	//}
	internal class Node<T> {
		public long nodeID;
		public Node<T> Parent;
		public List<Node<T>> Children;
		public T Value;

		public Node(T toAdd, long id) {
			this.Value = toAdd;
			this.nodeID = id;
		}
		// 자식이 없다면 리스트를 생성한뒤 추가.
		// 자식이 존재 한다면 단순히 리스트에 자식 추가.
		public bool AddChild(Node<T> toAdd) { 
			if (this.Children == null){
				this.Children = new List<Node<T>>{ 
					toAdd
				};
				return true;}
			else{
				this.Children.Add(toAdd);
				return true;
			}
		}
		public bool AddChild( Node<T> parent, Node<T> toAdd ) {
			if (this.Children == null){
				this.Children = new List<Node<T>>{ 
					toAdd
				};
				this.AddParent(parent);
				return true;
			}
			else{
				this.Children.Add(toAdd);
				this.AddParent(parent);
				return true;
			}
		}
		// 부모가 없으면 부모를 추가.
		// 부모가 있으면 추가 하지 않고 false를 반환.
		public bool AddParent(Node<T> toAdd) {
			if (toAdd == null) {
				this.Parent = null;
			}
			if (this.Parent == null){
				this.Parent = toAdd;
				return true;
			}
			return false;
		}
	}
	internal class CustomItem {
		public long id;
		private string title;
		public string Title { get { return this.title; } set { this.title = value; } }
		public UnitType unitType;
		public ItemCategory itemCategory;
		public ItemType itemType;
		public string code;
		public string specification;
		public bool parentsCheck;
		public int safetyStock;
		private string note;
		public string Note { get { return this.note;  } set {this.note = value; } }		//MES 에서 사용하는 변수 매핑 편의 위해 넣어둠
		public double quantity;
		public double inspectionRate;
		public int totalInventoryQuatity;

		public CustomItem() { }

		public CustomItem(long id, string title) {
			this.id = id;
			this.title = title;
		}

		public CustomItem(long id, string title, UnitType unitType, ItemCategory itemCategory, string note, double quantity) { 
				this.id = id;
			this.title = title;	
			this.unitType = unitType;	
			this.itemCategory = itemCategory;
			this.note = note;
			this.quantity = quantity;
		
		}

		public CustomItem(long id, string title, UnitType unitType, ItemCategory itemCategory, ItemType itemType, string code, string specification, bool parentCheck, int safetyStock, string note, double queatity) { 
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
