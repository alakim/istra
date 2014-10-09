<?php
	
	include 'doctype.php';

	echo('<h1>Publishing...</h1>');
	
	$xsltSettings = array(
		"contentFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["ContentFolder"],
		"cacheFolder"=>"../".$Settings["ThisFolder"]."/".$Settings["CacheFolder"]
	);
	
	$tocdoc = new DOMDocument();
	$tocdoc->load($Settings["ContentFolder"]."/toc.xml");
	$xpath = new DOMXpath($tocdoc);
	$sections = $xpath->query('//section[@file]');
	
	foreach($sections as $sect){
		$file = $sect->getAttribute("file");
		$outFile = preg_replace('/xml$/i', "html", $file);
		$html = xml2html($Settings["ContentFolder"]."/pages/".$file, $Settings["XsltFolder"]."/article.xslt", $xsltSettings);
		$html = setDocType($html);
		file_put_contents($TargetFolder."/$outFile", $html);
	}
	
	buildMenu();

		
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
	
	echo("<h2>DONE.</h2>");

?>
