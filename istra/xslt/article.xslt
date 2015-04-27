<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes"/>
	
	<xsl:variable name="debugMode">
		<xsl:if test="/article/@debug='true'">true</xsl:if>
	</xsl:variable>
	
	<xsl:variable name="tocPath"><xsl:value-of select="/article/@contentFolder"/>/toc.xml</xsl:variable>
	<xsl:variable name="menuPath"><xsl:value-of select="/article/@cacheFolder"/>/menu.xml</xsl:variable>
	<xsl:variable name="jsPath"><xsl:value-of select="/article/@jsFolder"/></xsl:variable>
	<xsl:variable name="cssPath"><xsl:value-of select="/article/@cssFolder"/></xsl:variable>
	
	<xsl:variable name="tocdoc" select="document($tocPath)"/>
	<xsl:variable name="SiteTitle" select="$tocdoc/toc/@title"/>

	<xsl:template match="/article">
		<xsl:variable name="menuWidth">350</xsl:variable>
		<xsl:variable name="padding">5</xsl:variable>
		<xsl:variable name="minHeight">700</xsl:variable>
		<xsl:variable name="currentPage" select="@page"/>
		
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html;charset=utf-8"/>
				<title><xsl:value-of select="$SiteTitle"/></title>
				<link rel="stylesheet" type="text/css" href="{$cssPath}styles.css"/>
				<script type="text/javascript" src="{$jsPath}/lib/html.js"></script>
				<script type="text/javascript" src="{$jsPath}/lib/jquery-1.11.0.min.js"></script>
				<script type="text/javascript" src="{$jsPath}/menu.js"></script>
				<xsl:choose>
					<xsl:when test="$debugMode='true'">
						<script type="text/javascript" src="js/debugView.js"></script>
					</xsl:when>
					<xsl:otherwise>
					</xsl:otherwise>
				</xsl:choose>
			</head>
			<body>
				<xsl:if test="not(/article/@print='true')">
					<header>
						<h1><xsl:value-of select="$SiteTitle"/></h1>
					</header>
				</xsl:if>
				<xsl:if test="not(/article/@print='true')">
					<div class="leftMenu" style="width:{$menuWidth}px; min-height:{$minHeight}px; padding:{$padding}px;">
						<xsl:call-template name="menu"/>
						<p><a href="?print&amp;p={$currentPage}">Версия для печати</a></p>
					</div>
				</xsl:if>
				<xsl:variable name="contentMargin">
					<xsl:choose>
						<xsl:when test="not(/article/@print='true')"><xsl:value-of select="$menuWidth+$padding*2"/></xsl:when>
						<xsl:otherwise>0</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<div class="mainPanel" style="margin-left:{$contentMargin}px; padding:{$padding*2}px;">
					<h2><xsl:value-of select="@title"/></h2>
					<ol><xsl:apply-templates select="section" mode="toc"/></ol>
					<xsl:apply-templates />
				</div>
				<div style="clear:left;"></div>

				<xsl:if test="not(/article/@print='true')">
					<footer>Istra Team <xsl:text disable-output-escaping="yes">&amp;copy;</xsl:text>2014</footer>
				</xsl:if>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="section" mode="toc">
		<li>
			<xsl:variable name="sectRef">
				<xsl:choose>
					<xsl:when test="@id"><xsl:value-of select="@id"/></xsl:when>
					<xsl:otherwise>s<xsl:value-of select="count(preceding::section)"/></xsl:otherwise>
				</xsl:choose>
			</xsl:variable>
			<a href="#{$sectRef}">
				<xsl:value-of select="@title"/>
				<xsl:if test="section">
					<ol><xsl:apply-templates select="section" mode="toc"/></ol>
				</xsl:if>
			</a>
		</li>
	</xsl:template>
		
	<xsl:template name="menu">
		
		<xsl:variable name="doc" select="document($menuPath)"/>
		<ul>
			<xsl:apply-templates select="$doc/menu/*" mode="menu"/>
		</ul>
	</xsl:template>
	
	<xsl:template match="link" mode="menu">
		<li><a href="{@url}"><xsl:value-of select="@title"/></a></li>
	</xsl:template>
	
	<xsl:template match="section" mode="menu">
		<xsl:variable name="file">
			<xsl:choose>
				<xsl:when test="@file"><xsl:value-of select="@file"/></xsl:when>
				<xsl:otherwise><xsl:value-of select="ancestor::section[@file]/@file"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<li>
			<xsl:choose>
				<xsl:when test="@file or @anchor">
					<xsl:choose>
						<xsl:when test="@noSef='true'">
							<a href="test2.aspx/?p={$file}#{@anchor}"><xsl:value-of select="@title"/></a>
						</xsl:when>
						<xsl:otherwise>
							<a href="/{$file}.html#{@anchor}"><xsl:value-of select="@title"/></a>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:otherwise><span class="menuSection"><xsl:value-of select="@title"/></span></xsl:otherwise>
			</xsl:choose>
			
			<xsl:if test="section">
				<ul>
					<xsl:apply-templates select="section" mode="menu"/>
				</ul>
			</xsl:if>
		</li>
	</xsl:template>
	
	<xsl:template match="section">
		<xsl:variable name="sID">
			<xsl:choose>
				<xsl:when test="@id"><xsl:value-of select="@id"/></xsl:when>
				<xsl:otherwise>s<xsl:value-of select="count(preceding::section)+count(ancestor::section)"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="level" select="count(ancestor::section)+3"/>
		<xsl:variable name="sNum" select="count(preceding-sibling::section)+1"/>
		<a name="{$sID}"></a>
		<xsl:element name="h{$level}">
			<xsl:for-each select="ancestor-or-self::section">
				<xsl:value-of select="count(preceding-sibling::section)+1"/>.
			</xsl:for-each>
			<xsl:value-of select="@title"/>
		</xsl:element>
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:include href="tags.xslt"/>
	
	<xsl:include href="../../overrides/tags.xslt"/>
	<xsl:include href="../../overrides/templates.xslt"/>
	
	<xsl:template match="*">
		<div style="border:1px solid #f00; padding:3px; background-color:#ffe; color:#f00;">
			<p>Unknown Tag '<xsl:value-of select="name()"/>'</p>
			<p>Attributes:</p>
			<ul>
				<xsl:for-each select="@*">
					<li><xsl:value-of select="name()"/> = '<xsl:value-of select="."/>'</li>
				</xsl:for-each>
			</ul>
		</div>
	</xsl:template>
	
</xsl:stylesheet>
