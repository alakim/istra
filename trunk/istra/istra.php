<?php
	
	$clearCache = false;
	
	if(isset($_REQUEST['clearcache'])){
		$clearCache = true;
	}
	if(isset($_REQUEST['p'])){
		$page = $_REQUEST["p"];
	}
	if(isset($_REQUEST['debug'])){
		$debugMode = true;
	}
	if(empty($page)){
		$toc = new DOMDocument();
		$toc->load($Settings["ContentFolder"]."/toc.xml");
		$sect = $toc->getElementsByTagName('section')->item(0);
		$file = explode(".", $sect->getAttribute("file"));
		$page = $file[0];
	}
	
	function buildMenu(){
		global $Settings;
		$menu = xml2html($Settings["ContentFolder"]."/toc.xml", $Settings["XsltFolder"]."/menu.xslt", array(
			"contentFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["ContentFolder"],
			"cacheFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["CacheFolder"]
		));
		file_put_contents($Settings["CacheFolder"]."/menu.xml", $menu);
	}

	function xml2html($xmlcontent, $xsl, $settings){
		global $debugMode;
		
		$xmlDoc = new DOMDocument();
		$xmlDoc->load($xmlcontent);
		if($debugMode){
			$root = $xmlDoc->documentElement;
			$root->setAttribute("debug", "true");
		}
		
		foreach(array_keys($settings) as $k){
			$v = $settings[$k];
			$root = $xmlDoc->documentElement;
			$root->setAttribute($k, $v);
		}
		
		$xslDoc = new DOMDocument();
		$xslDoc->load($xsl);

		$proc = new XSLTProcessor();
		$proc->importStylesheet($xslDoc);
		return $proc->transformToXML($xmlDoc);
	}
	
	if($clearCache || !file_exists($Settings["CacheFolder"]."/menu.xml"))
		buildMenu();

	if($page){
		echo(xml2html($Settings["ContentFolder"]."/pages/".$page.".xml", $Settings["XsltFolder"]."/article.xslt", array(
			"contentFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["ContentFolder"],
			"cacheFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["CacheFolder"]
		)));
	}

?>
