(function($, $H){
	
	var typeDefinition;
	
	function getTypeDefinition(typeOrNode){
		var type = typeof(typeOrNode)=="string"?typeOrNode:xType(typeOrNode);
		return typeDefinition&&typeDefinition[type];
	}
	function getAttrDefinitions(xNd){
		var def = getTypeDefinition(xNd);
		return def && def.attributes;
	}
	function getChildrenDefinitions(xNd){
		var def = getTypeDefinition(xNd);
		return def && def.children;
	}
	function getTypeAlias(nameOrNode){
		var tName = typeof(nameOrNode)=="object"?xType(nameOrNode):nameOrNode;
		var def = typeDefinition&&typeDefinition[tName];
		return def&&def.alias || xType(nameOrNode);
	}
	
	function xType(xNd, t){if(t){xNd._type = t;} return typeof(xNd)=="object"?xNd._type:"xmlText";}
	function xAttributes(xNd, coll){if(coll){xNd._attr = coll;} return xNd._attr;}
	function xChildren(xNd, coll){if(coll){xNd._ch = coll;} return xNd._ch;}
	
	function getCount(xNd, childType){
		var res = 0;
		var coll = xChildren(xNd);
		if(!coll) return 0;
		for(var i=0,ch; ch=coll[i],i<coll.length; i++){
			if(xType(ch)==childType) res++;
		}
		return res;
	}
	
	function getAvailableChildren(xNd){
		var res = [];
		var defColl = getTypeDefinition(xNd);
		if(defColl){
			for(var nm in defColl.children){
				var def = defColl.children[nm];
				if(!def) continue;
				var count = getCount(xNd, nm);
				if(def.count instanceof(Array)){
					if(!def.count[1] || def.count[1]>count) res.push(nm);
				}
				else if(count<def.count){
					res.push(nm);
				}
			}
		}
		return res.length?res:null;
	}
	
	var templates = {
		main: function(data){with($H){
			return div(
				h3("XML Editor"),
				templates.xNode(data),
				div(
					input({type:"button", value:"Сохранить", "class":"btSave"})
				)
			);
		}},
		xAttribute: function(xNd, attNm, def, attVal){with($H){
			return div({"class":"xAttribute", xAttribute:attNm},
				span({"class":"attrName"+(def&&def.mandatory?" mandatory":"")}, 
					(def&&def.alias)||attNm
				), ": ",
				input({type:"text", "class":"attrValue", value:attVal||""}),
				templates.star(def&&def.mandatory)
			);
		}},
		xNode: function(xNd, mandatory){with($H){
			var attrs = xAttributes(xNd),
				attrDefs = getAttrDefinitions(xNd),
				childDefs = getChildrenDefinitions(xNd),
				availableChildren = getAvailableChildren(xNd);
			return div({"class":"xNode", xType:xType(xNd)},
				(typeof(xNd)=="string" || xType(xNd)=="xmlText")?div(
					textarea({"class":"textNode"}, xNd.text || xNd),
					templates.star(mandatory)
				)
				:typeof(xNd)=="object"? markup(
					div({"class":"nodeType"},
						getTypeAlias(xNd),
						templates.star(mandatory),
						input({type:"button", "class":"btDelNode", value:"Удалить"})
					),
					div({"class":"nodeAttributes"},
						apply(attrs, function(v, k){
							return templates.xAttribute(xNd, k, attrDefs[k], v);
						}),
						apply(attrDefs, function(def, attNm){
							if(!(attrs && attrs[attNm])){ 
								return templates.xAttribute(xNd, attNm, attrDefs[attNm]);
							}
						})
					),
					div({"class":"nodeChildren"},
						apply(xChildren(xNd), function(ch){
							var cDef = childDefs[xType(ch)];
							return templates.xNode(ch, cDef&&cDef.mandatory);
						})
					),
					div({"class":"nodeButtons"},
						availableChildren?markup(
							input({type:"button", "class":"btAddNode", value:"Добавить"}),
							select({"class":"selNodeType"},
								apply(availableChildren, function(nm){
									return option({value:nm}, getTypeAlias(nm))
								})
							)
						):null
					)
				)
				:typeof(xNd)=="numeric"?input({type:"text", value:xNd})
				:div({"class":"error"}, "Unknown node type "+typeof(xNd))
			);
		}},
		star: function(mandatory){with($H){
			return mandatory?span({"class":"star"}, "*"):null;
		}}
	};
	
	function collectData(panel){
		function buildNode(nd){nd=$(nd);
			var res = {};
			xType(res, nd.attr("xType"));
			
			var attrs = {}, attrsFound = false;
			
			var attColl = $(nd[0]).children(".nodeAttributes").children(".xAttribute")
			$.each(attColl, function(i, att){att=$(att);
				var attVal = att.find(".attrValue").val();
				if(attVal&&attVal.length){
					attrs[att.attr("xAttribute")] = attVal;
					attrsFound = true;
				}
			});
			if(attrsFound) xAttributes(res, attrs);
			
			var children = [];
			$.each($(nd.find(".nodeChildren")[0]).children(), function(i, ch){
				if($(ch).attr("xType")) children.push(buildNode(ch));
			});
			if(children.length) xChildren(res, children);
			if(xType(res)=="xmlText"){
				res.text = nd.find("textarea.textNode").val();
			}
			return res;
		}
		
		return buildNode(panel.find(".xNode"));
	}
	
	function formatString(str){
		return str.replace(/\&/g, "&amp;")
			.replace(/</g, "&lt;")
			.replace(/>/g, "&gt;")
			.replace(/\"/g, "&quot;")
			.replace(/\'/g, "&apos;");
	}
	function serialize(nd){
		if(typeof(nd)=="string") return formatString(nd);
		if(xType(nd)=="xmlText") return formatString(nd.text);
		var res = [];
		res.push("<"+xType(nd));
		var attributes = xAttributes(nd);
		for(var nm in attributes){
			var v = attributes[nm];
			res.push(" "+nm+"=\""+formatString(v)+"\"");
		}
		var children = xChildren(nd);
		if(children&&children.length){
			res.push(">");
			$.each(children, function(i, ch){
				res.push(serialize(ch));
			});
			res.push("</"+xType(nd)+">");
		}
		else res.push("/>");
		return res.join("");
	}
	
	function createEmptyNode(ndType){
		if(ndType=="xmlText") return "";
		var nd = {},
			def = getTypeDefinition(ndType),
			chColl = [];
		xType(nd, ndType);
		xChildren(nd, chColl);
		for(var t in def.children){
			var chDef = def.children[t];
			if(chDef.mandatory){
				chColl.push(createEmptyNode(t));
			}
		}
		return nd;
	}
	
	function init(panel, data, onsave){
		panel.html(templates.main(data));
		panel.find(".btSave").click(function(){
			var res = collectData(panel);
			var xml = serialize(res);
			if(onsave) onsave(xml);
		});
		panel.find(".btAddNode").click(function(){
			var type = $(this).parent().find(".selNodeType").val();
			var nd = createEmptyNode(type);
			chPnl = $(this).parent().parent().children(".nodeChildren")[0];
			$(chPnl).append(templates.xNode(nd));
			var nNd = collectData(panel);
			init(panel, nNd, onsave);
		});
		panel.find(".btDelNode").click(function(){
			var pnl = $(this).parent().parent();
			if(confirm([
				"Удалить элемент  \"",
				getTypeAlias(pnl.attr("xType")).toLowerCase(),
				"\"?"
			].join(""))){
				var pp = pnl.parent().parent();
				pnl.remove();
				var nNd = collectData(panel);
				init(panel, nNd, onsave);
			}
		});
	}
	
	$.fn.xmlEditor = function(data, def, onsave){
		typeDefinition = def;
		$(this).each(function(i, el){
			init($(el), data, onsave);
		});
	};
	

	
})(jQuery, Html);