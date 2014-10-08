<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:istra="http://www.istra.com/cms">
	<xsl:output method="html" version="1.0" encoding="utf-8" indent="yes"/>
	


	<xsl:template match="/pictures">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html;charset=utf-8"/>
				<link rel="stylesheet" type="text/css" href="styles.css"/>
			</head>
			<body>
				<h1>Pictures registry</h1>
				<h2>Numbered pictures</h2>
				<table border="1" cellpadding="3" cellspacing="0">
					<tr>
						<th width="30" valign="top">#</th>
						<th width="100" valign="top">ID</th>
						<th width="100" valign="top">File</th>
						<th width="400" valign="top">Description</th>
						<th width="400" valign="top">Image</th>
					</tr>
					<xsl:apply-templates select="pict"/>
				</table>
				<h2>Inline images</h2>
				<table border="1" cellpadding="3" cellspacing="0">
					<tr>
						<th width="100" valign="top">ID</th>
						<th width="100" valign="top">File</th>
						<th width="400" valign="top">Description</th>
						<th width="400" valign="top">Image</th>
					</tr>
					<xsl:apply-templates select="img"/>
				</table>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="pict">
		<tr>
			<td valign="top"><xsl:value-of select="count(preceding::pict)+1"/></td>
			<td valign="top"><xsl:value-of select="@id"/></td>
			<td valign="top"><xsl:value-of select="@file"/></td>
			<td valign="top"><xsl:value-of select="text()"/></td>
			<td valign="top"><img width="400" src="{/pictures/@baseUrl}/{@file}"/></td>
		</tr>
	</xsl:template>
	
	<xsl:template match="img">
		<tr>
			<td valign="top"><xsl:value-of select="@id"/></td>
			<td valign="top"><xsl:value-of select="@file"/></td>
			<td valign="top"><xsl:value-of select="text()"/></td>
			<td valign="top"><img src="{/pictures/@baseUrl}/{@file}"/></td>
		</tr>
	</xsl:template>
	
</xsl:stylesheet>
