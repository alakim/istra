<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes"/>
	
	<xsl:variable name="debugMode">
		<xsl:if test="/article/@debug='true'">true</xsl:if>
	</xsl:variable>
	
	<xsl:variable name="tocPath"><xsl:value-of select="/article/@contentFolder"/>/toc.xml</xsl:variable>
	<xsl:variable name="menuPath"><xsl:value-of select="/article/@cacheFolder"/>/menu.xml</xsl:variable>
	
	<xsl:variable name="tocdoc" select="document($tocPath)"/>
	<xsl:variable name="SiteTitle" select="$tocdoc/toc/@title"/>

	<xsl:template match="/article">
		
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html;charset=utf-8"/>
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
				<h1><xsl:value-of select="$SiteTitle"/></h1>
				<table border="0" cellpadding="3" cellspacing="0">
					<tr>
						<td width="300" id="menuPnl" valign="top">
							<xsl:call-template name="menu"/>
						</td>
						<td valign="top">
							<h2><xsl:value-of select="@title"/></h2>
							<xsl:apply-templates />
						</td>
					</tr>
				</table>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template name="menu">
		
		<xsl:variable name="doc" select="document($menuPath)"/>
		<ul>
			<xsl:apply-templates select="$doc/menu/section" mode="menu"/>
		</ul>
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
					<a href="?p={$file}#{@anchor}"><xsl:value-of select="@title"/></a>
				</xsl:when>
				<xsl:otherwise><xsl:value-of select="@title"/></xsl:otherwise>
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
		<a name="{$sID}"></a>
		<xsl:element name="h{$level}"><xsl:value-of select="@title"/></xsl:element>
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:include href="tags.xslt"/>
	
	<xsl:include href="overrides/tags.xslt"/>
	<xsl:include href="overrides/templates.xslt"/>
	
</xsl:stylesheet>
