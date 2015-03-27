(function($, $H){
	
	function init(menu){
		var top = menu.find(">ul>li");
		top.find("ul").addClass("subMenu").hide();
		top.mouseover(function(){
			$(this).find("ul").show();
		}).mouseout(function(){
			$(this).find("ul").hide();
		});
	}
	
	$.fn.topMenu = function(){
		$(this).each(function(i, el){
			init($(el));
		});
	};
	
	$(function(){
		$(".topMenu").topMenu();
	});
	
})(jQuery, Html);