<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes"/>
	


	<xsl:template match="/">
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="directory">
		<dir name="{@name}">
			<xsl:apply-templates/>
		</dir>
	</xsl:template>
	
	<xsl:template match="file">
		<file name="{@name}"/>
	</xsl:template>
	
	

	
</xsl:stylesheet>
