<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes"/>
	

	<xsl:template match="p">
		<p><xsl:apply-templates/></p>
	</xsl:template>
	
	<xsl:template match="ref">
		<xsl:choose>
			<xsl:when test="@mail">
				<a href="mailto:{@mail}">
					<xsl:choose>
						<xsl:when test="text()"><xsl:value-of select="text()"/></xsl:when>
						<xsl:otherwise><xsl:value-of select="@mail"/></xsl:otherwise>
					</xsl:choose>
				</a>
			</xsl:when>
			<xsl:when test="@book">
				[<a href="#{@book}"><xsl:value-of select="@book"/></a>]
			</xsl:when>
			<xsl:when test="@website">
				[<a href="#{@website}"><xsl:value-of select="@website"/></a>]
			</xsl:when>
			<xsl:when test="@url">
				<a href="{@url}">
					<xsl:choose>
						<xsl:when test="text()"><xsl:value-of select="text()"/></xsl:when>
						<xsl:otherwise><xsl:value-of select="@url"/></xsl:otherwise>
					</xsl:choose>
				</a>
			</xsl:when>
			<xsl:when test="@pict">
				<xsl:variable name="pictID" select="@pict"/>
				рис.<xsl:value-of select="count(document('../../content/pictures.xml')/pictures/pict[@id=$pictID]/preceding::pict)+1"/>
			</xsl:when>
			<xsl:when test="@sect">
				<xsl:variable name="pageID" select="substring-before(@sect, '#')"/>
				<xsl:variable name="sectID" select="substring-after(@sect, '#')"/>
				<xsl:variable name="pageRef">
					<xsl:choose>
						<xsl:when test="string-length($pageID)=0"><xsl:value-of select="/article/@page"/></xsl:when>
						<xsl:otherwise><xsl:value-of select="$pageID"/></xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				
				<a href="?p={$pageRef}#{$sectID}"><xsl:apply-templates/></a>
			</xsl:when>
			<xsl:otherwise><xsl:apply-templates/></xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template match="attention"><div class="attention"><xsl:apply-templates/></div></xsl:template>
	<xsl:template match="comment"><div class="comment"><xsl:apply-templates/></div></xsl:template>

	<xsl:template match="pict">
		<div style="text-align:center;">
			<xsl:variable name="pictID" select="@id"/>
			<xsl:variable name="registry" select="document('../../content/pictures.xml')"/>
			<xsl:variable name="pict" select="$registry/pictures/pict[@id=$pictID]"/>
			<img src="{$registry/pictures/@baseUrl}/{$pict/@file}"/>
			<p>Рис. <xsl:value-of select="count($pict/preceding::pict)+1"/> - <xsl:value-of select="$pict/text()"/></p>
		</div>
	</xsl:template>

	<xsl:template match="img">
		<xsl:variable name="imgID" select="@id"/>
		<xsl:variable name="registry" select="document('../../content/pictures.xml')"/>
		<xsl:variable name="img" select="$registry/pictures/img[@id=$imgID]"/>
		<img src="{$registry/pictures/@baseUrl}/{$img/@file}"/>
	</xsl:template>
	
	<xsl:template match="menu">
		<span class="menuName"><xsl:apply-templates/></span>
	</xsl:template>
	
	<xsl:template match="button">
		<span class="buttonName"><xsl:apply-templates/></span>
	</xsl:template>
	
	<xsl:template match="key">
		<span class="keyName"><xsl:apply-templates/></span>
	</xsl:template>
	
	<xsl:template match="list">
		<xsl:variable name="tag">
			<xsl:choose>
				<xsl:when test="@marker='num'">ol</xsl:when>
				<xsl:otherwise>ul</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:if test="caption">
			<xsl:apply-templates select="caption" mode="outside"/>
		</xsl:if>
		<xsl:element name="{$tag}">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="li">
		<li>
			<xsl:if test="@name"><span class="liName"><xsl:value-of select="@name"/></span> - </xsl:if>
			<xsl:apply-templates/>
		</li>
	</xsl:template>
	
	<xsl:template match="caption"/>
	<xsl:template match="caption" mode="outside">
		<p class="listCaption"><xsl:apply-templates/></p>
	</xsl:template>
	
	<xsl:template match="sel"><span class="selected"><xsl:apply-templates/></span></xsl:template>
	<xsl:template match="strong"><strong><xsl:apply-templates/></strong></xsl:template>
	<xsl:template match="file"><span class="file"><xsl:apply-templates/></span></xsl:template>
	<xsl:template match="code"><span class="code"><xsl:apply-templates/></span></xsl:template>
	<xsl:template match="codesample"><div class="code"><pre><xsl:apply-templates/></pre></div></xsl:template>
	
	<xsl:template match="patch">
		<xsl:variable name="patch" select="document(@file)"/>
		<xsl:apply-templates select="$patch/patch/*"/>
	</xsl:template>
	
	<xsl:template match="todo">
		<xsl:if test="$debugMode='true'">
			<div class="todo"><xsl:apply-templates/></div>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="book">
		<div>
			<a name="{@id}">
				[<xsl:value-of select="@id"/>] <xsl:value-of select="@author"/>. <xsl:value-of select="@title"/>. <xsl:value-of select="@pub"/>
			</a>
		</div>
	</xsl:template>
	
	<xsl:template match="webArticle">
		<div>
			<a name="{@id}">
				[<xsl:value-of select="@id"/>] <a href="{@url}"><xsl:value-of select="@title"/></a>
			</a>
		</div>
	</xsl:template>
	
	
</xsl:stylesheet>
