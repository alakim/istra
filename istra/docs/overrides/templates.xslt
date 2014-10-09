<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes"/>
	

	
	<xsl:template match="/article">
		<xsl:variable name="menuWidth">350</xsl:variable>
		<xsl:variable name="padding">5</xsl:variable>
		<xsl:variable name="minHeight">700</xsl:variable>
		
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html;charset=utf-8"/>
				<title><xsl:value-of select="$SiteTitle"/></title>
				<link rel="stylesheet" type="text/css" href="styles.css"/>
				<script type="text/javascript" src="js/lib/html.js"></script>
				<script type="text/javascript" src="js/lib/jquery-1.11.0.min.js"></script>
				<xsl:choose>
					<xsl:when test="$debugMode='true'">
						<script type="text/javascript" src="js/debugView.js"></script>
					</xsl:when>
					<xsl:otherwise>
					</xsl:otherwise>
				</xsl:choose>
			</head>
			<body>
				<header>
					<h1><xsl:value-of select="$SiteTitle"/></h1>
				</header>
				<div class="leftMenu" style="width:{$menuWidth}px; min-height:{$minHeight}px; padding:{$padding}px;">
					<xsl:call-template name="menu"/>
				</div>
				<div class="mainPanel" style="margin-left:{$menuWidth+$padding*2}px; padding:{$padding*2}px;">
					<h2><xsl:value-of select="@title"/></h2>
					<xsl:apply-templates />
				</div>
				<div style="clear:left;"></div>
				<footer>Istra Team <xsl:text disable-output-escaping="yes">&amp;copy;</xsl:text>2014</footer>
			</body>
		</html>
	</xsl:template>
	
	
	
</xsl:stylesheet>
