(function($, $H){
	
	var typeDefinition;
	
	function getTypeDefinition(xNd){
		return typeDefinition&&typeDefinition[xNd._type];
	}
	function getAttrDefinitions(xNd){
		var def = getTypeDefinition(xNd);
		return def && def.attributes;
	}
	function getChildrenDefinitions(xNd){
		var def = getTypeDefinition(xNd);
		return def && def.children;
	}
	
	function xType(xNd, t){if(t){xNd._type = t;} return typeof(xNd)=="object"?xNd._type:"xmlText";}
	function xAttributes(xNd, coll){if(coll){xNd._attr = coll;} return xNd._attr;}
	function xChildren(xNd, coll){if(coll){xNd._ch = coll;} return xNd._ch;}
	
	
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
				childDefs = getChildrenDefinitions(xNd);
			return div({"class":"xNode", xType:xType(xNd)},
				typeof(xNd)=="object"? markup(
					div({"class":"nodeType"},
						xType(xNd),
						templates.star(mandatory)
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
					)
				)
				:typeof(xNd)=="string"?div(
					textarea({"class":"textNode"}, xNd),
					templates.star(mandatory)
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
				children.push(buildNode(ch));
			});
			if(children.length) xChildren(res, children);
			if(xType(res)=="xmlText"){
				res.text = nd.find("textarea.textNode").val();
			}
			return res;
		}
		
		var res = buildNode(panel.find(".xNode"));
		console.log(res);
		return res;
	}
	
	function formatString(str){
		return str.replace(/\&/g, "&amp;")
			.replace(/</g, "&lt;")
			.replace(/>/g, "&gt;")
			.replace(/\"/g, "&quot;")
			.replace(/\'/g, "&apos;");
	}
	function serialize(nd){
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
	
	function init(panel, data){
		panel.html(templates.main(data));
		panel.find(".btSave").click(function(){
			var res = collectData(panel);
			var xml = serialize(res);
			console.log("Saved: ", xml);
		});
	}
	
	$.fn.xmlEditor = function(data, def){
		typeDefinition = def;
		$(this).each(function(i, el){
			init($(el), data);
		});
	};
	

	
})(jQuery, Html);