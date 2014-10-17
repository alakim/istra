(function($){
	$.fn.accordionMenu = function(){
		$.each(this, function(i, el){var el=$(el);
			el.find("ul").hide();
			el.find("li").click(function(){var _=$(this);
				_.find(">ul").show();
			});
		});
	};
	
	$(function(){
		var url = document.location.href;
		var mt = url.match(/p=([^#]+)/);
		if(!mt) return;
		var page = mt[1];
		var re = new RegExp(page, "g");
		
		$(".leftMenu li").each(function(i, el){el=$(el);
			el.find("a").each(function(i, a){a=$(a);
				if(a.attr("href").match(re)){
					el.addClass("current");
					//console.log("current ", a[0]);
				}
			});
		});
		$(".leftMenu ul").accordionMenu();
		$(".current").each(function(i,el){
			function openParent(itm){
				var prt = itm.parent();
				if(!prt.length) return;
				prt.show();
				openParent(prt);
			}
			openParent($(el));
		});
	});
})(jQuery);