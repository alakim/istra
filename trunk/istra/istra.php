<?php
	
	include 'doctype.php';
	
	$clearCache = isset($_REQUEST['clearcache']);
	
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
	
	$xsltSettings = array(
		"contentFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["ContentFolder"],
		"cacheFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["CacheFolder"]
	);
		
	function buildMenu(){
		global $Settings;
		global $xsltSettings;
		$menu = xml2html($Settings["ContentFolder"]."/toc.xml", $Settings["XsltFolder"]."/menu.xslt", $xsltSettings);
		file_put_contents($Settings["CacheFolder"]."/menu.xml", $menu);
	}

	function xml2html($xmlcontent, $xsl, $settings){
		global $debugMode;
		global $Settings;
		
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
		
		// подменить пути overrides
		$xpath = new DOMXpath($xslDoc);
		$includes = $xpath->query("//xsl:include");
		foreach($includes as $inc){
			$ref = $inc->getAttribute("href");
			$newRef = preg_replace('/overrides/i', '../'.$Settings["ThisFolder"].'/'.$Settings["OverridesFolder"], $ref);
			$inc->setAttribute("href", $newRef);
		}
		

		$proc = new XSLTProcessor();
		$proc->importStylesheet($xslDoc);
		return $proc->transformToXML($xmlDoc);
	}
	
	if($clearCache || !file_exists($Settings["CacheFolder"]."/menu.xml"))
		buildMenu();

	if($page){
		$html = xml2html($Settings["ContentFolder"]."/pages/".$page.".xml", $Settings["XsltFolder"]."/article.xslt", $xsltSettings);
		$html = setDocType($html);
		// ob_start();
		// ob_clean();
		echo($html);
	}
