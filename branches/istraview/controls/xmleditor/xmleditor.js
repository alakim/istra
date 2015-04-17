(function($, $H){
	
	var typeDefinition;
	
	function getType(xNd){
		var def = typeDefinition&&typeDefinition[xNd._type];
		return def&&def.alias?def.alias:xNd._type;
	}
	function getAttributes(xNd){return xNd._attr;}
	function getChildren(xNd){return xNd._ch;}
	function getAttributeName(xNd, attrNm){
		var def = typeDefinition&&typeDefinition[xNd._type];
		if(!(def && def.attributes)) return attrNm;
		var attrDef = def.attributes[attrNm];
		return attrDef&&attrDef.alias?attrDef.alias:attrNm;
	}
	
	var templates = {
		main: function(data){with($H){
			return div(
				h3("XML Editor"),
				templates.xNode(data)
			);
		}},
		xNode: function(xNd){with($H){
			return div({"class":"xNode"},
				typeof(xNd)=="object"? markup(
					div({"class":"nodeType"}, getType(xNd)),
					div({"class":"nodeAttributes"},
						apply(getAttributes(xNd), function(v, k){
							return div(
								span({"class":"attrName"}, getAttributeName(xNd, k)), ": ",
								input({type:"text", "class":"attrValue", value:v})
							)
						})
					),
					div({"class":"nodeChildren"},
						apply(getChildren(xNd), function(ch){
							return templates.xNode(ch);
						})
					)
				)
				:typeof(xNd)=="string"?textarea({"class":"textNode"}, xNd)
				:typeof(xNd)=="numeric"?input({type:"text", value:xNd})
				:div({"class":"error"}, "Unknown node type "+typeof(xNd))
			);
		}}
	};
	
	function init(panel, data){
		console.log(data);
		panel.html(templates.main(data));
	}
	
	$.fn.xmlEditor = function(data, def){
		console.log(def);
		typeDefinition = def;
		$(this).each(function(i, el){
			init($(el), data);
		});
	};
	

	
})(jQuery, Html);