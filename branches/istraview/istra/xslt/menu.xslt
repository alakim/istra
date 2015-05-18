<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes"/>
	

	
	<xsl:variable name="fileBasePath"><xsl:value-of select="/toc/@contentFolder"/></xsl:variable>

	<xsl:template match="/toc">
		<menu>
			<xsl:if test="@root"><xsl:attribute name="root"><xsl:value-of select="@root"/></xsl:attribute></xsl:if>
			<xsl:apply-templates/>
			<xsl:if test="@debug='true'">
				<xsl:apply-templates mode="debug"/>
			</xsl:if>
		</menu>
	</xsl:template>
	
	
	<xsl:template match="link">
		<link url="{@url}" title="{@title}"/>
	</xsl:template>
	
	<xsl:template match="section[@title]">
		<section title="{@title}">
			<xsl:apply-templates />
		</section>
	</xsl:template>
	
	<xsl:template match="section">
		<xsl:variable name="file"><xsl:value-of select="$fileBasePath"/>/pages/<xsl:value-of select="@file"/></xsl:variable>
		<xsl:variable name="doc" select="document($file)"/>
		<xsl:variable name="id" select="substring-before(@file, '.')"/>
		<xsl:variable name="title" select="$doc/article/@title"/>
		<section file="{$id}" title="{$title}">
			<xsl:if test="@hidden"><xsl:attribute name="hidden"><xsl:value-of select="@hidden"/></xsl:attribute></xsl:if>
			<xsl:if test="@noSef='true'"><xsl:attribute name="noSef">true</xsl:attribute></xsl:if>
		
			<xsl:if test="not(@topLevelOnly='true')">
				<xsl:apply-templates select="$doc/article/section" mode="sub"/>
			</xsl:if>
			<xsl:apply-templates/>
		</section>
	</xsl:template>
		
				
	<xsl:template match="section" mode="sub">
		<xsl:variable name="sID">
			<xsl:choose>
				<xsl:when test="@id"><xsl:value-of select="@id"/></xsl:when>
				<xsl:otherwise>s<xsl:value-of select="count(preceding::section)+count(ancestor::section)"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<section anchor="{$sID}" title="{@title}">
			<xsl:apply-templates select="section" mode="sub"/>
		</section>
	</xsl:template>
	
	<xsl:template match="section[@title]" mode="debug">
		<xsl:apply-templates select="section" mode="debug"/>
	</xsl:template>
				
	<xsl:template match="section" mode="debug">
		<xsl:variable name="file"><xsl:value-of select="$fileBasePath"/>/pages/<xsl:value-of select="@file"/></xsl:variable>
		<xsl:variable name="doc" select="document($file)"/>
		
		<xsl:if test="$doc//todo">
			<todo id="{substring-before(@file, '.')}" message=""/>
		</xsl:if>
		<xsl:apply-templates select="section" mode="debug"/>
	</xsl:template>
	
</xsl:stylesheet>
