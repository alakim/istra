<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes"/>
	

	<!--xsl:template match="p">
		<p style="color:#f40;"><xsl:apply-templates/></p>
	</xsl:template-->
	
	
	
	
	<xsl:template match="news">
		<h3>NEWS LIST</h3>
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:template match="message">
		<p>
			<xsl:value-of select="@date"/>: 
			<xsl:value-of select="@title"/>
		</p>
	</xsl:template>
	
	
	<xsl:template match="session">
		<ul>
			<xsl:apply-templates mode="sessionView"/>
		</ul>
		<p>Всего <xsl:value-of select="count(*)"/> параметров.</p>
	</xsl:template>
	<xsl:template match="param" mode="sessionView">
		<li><xsl:value-of select="@name"/>: <xsl:value-of select="@value"/></li>
	</xsl:template>
	
	
	<xsl:template match="AuthDialog">
		<form action="{@target}" method="post">
			<div style="border:1px solid #ccc; padding:5px; margin:5px; width:290px; height:100px;">
				<div>Логин: <input type="text" name="login"/></div>
				<div>Пароль: <input type="password" name="password"/></div>
				<div><input type="submit" value="Ввод"/></div>
			</div>
		</form>
	</xsl:template>
	
	
	
	<xsl:template match="error">
		<div style="border:1px solid #f00; padding:5px; margin:5px; width:800px; background-color:#ffe; color:#f00;">
			<p style="font-weight:bold;">Ошибка приложения</p>
			<xsl:apply-templates/>
		</div>
	</xsl:template>
	
	
</xsl:stylesheet>
