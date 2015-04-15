<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes"/>
	
	<xsl:template match="*">
	  <xsl:copy>
		<xsl:copy-of select="@*"/>
		<xsl:apply-templates/>
	  </xsl:copy>
	</xsl:template>

</xsl:stylesheet>
