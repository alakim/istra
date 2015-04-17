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
				<link rel="stylesheet" type="text/css" href="/styles.css"/>
				<script type="text/javascript" src="/js/lib/html.js"></script>
				<script type="text/javascript" src="/js/lib/jquery-1.11.0.min.js"></script>
				<script type="text/javascript" src="/js/topmenu.js"></script>
				<xsl:choose>
					<xsl:when test="$debugMode='true'">
						<script type="text/javascript" src="/js/debugView.js"></script>
					</xsl:when>
					<xsl:otherwise>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:for-each select="//addScript">
					<script type="text/javascript" src="{@src}"> </script>
				</xsl:for-each>
				<xsl:for-each select="//addCSS">
					<link rel="stylesheet" type="text/css" href="{@src}"/>
				</xsl:for-each>
			</head>
			<body>
				<header>
					<div class="header pagewidth">
						<h1><xsl:value-of select="$SiteTitle"/></h1>
					</div>
				</header>
				<nav>
					<div class="topMenu pagewidth">
						<xsl:call-template name="menu"/>
					</div>
					<div style="clear:left;"></div>
				</nav>
				<div class="mainPanel pagewidth">
					<h2><xsl:value-of select="@title"/></h2>
					<xsl:apply-templates />
				</div>
				<div style="clear:left;"></div>
				<footer>
					<div class="pagewidth">
						<xsl:text disable-output-escaping="yes">&amp;copy;</xsl:text> 2015 ИВЦ <xsl:text disable-output-escaping="yes">&amp;laquo;</xsl:text>Мосстрой<xsl:text disable-output-escaping="yes">&amp;raquo;</xsl:text>
					</div>
				</footer>
			</body>
		</html>
	</xsl:template>
	
	
	<!--xsl:template match="section" mode="menu">
		<xsl:variable name="file">
			<xsl:choose>
				<xsl:when test="@file"><xsl:value-of select="@file"/></xsl:when>
				<xsl:otherwise><xsl:value-of select="ancestor::section[@file]/@file"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<li>
			<xsl:choose>
				<xsl:when test="@file or @anchor">
					<a href="/article/{$file}/#{@anchor}"><xsl:value-of select="@title"/></a>
				</xsl:when>
				<xsl:otherwise><span class="menuSection"><xsl:value-of select="@title"/></span></xsl:otherwise>
			</xsl:choose>
			
			<xsl:if test="section">
				<ul>
					<xsl:apply-templates select="section" mode="menu"/>
				</ul>
			</xsl:if>
		</li>
	</xsl:template-->

	
	<xsl:template match="addScript"></xsl:template>
	<xsl:template match="addCSS"></xsl:template>
	
</xsl:stylesheet>
