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
	
	
</xsl:stylesheet>
