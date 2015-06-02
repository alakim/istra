﻿<?xml version="1.0" encoding="UTF-8"?>
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
	
	
	<xsl:template match="querySample">
		<xsl:apply-templates mode="sample"/>
	</xsl:template>
	
	<xsl:template match="request" mode="sample">
		<div style="border:1px solid #ccc; margin:5px; padding:5px;">
			<p>Результаты запроса:</p>
			<ul>
				<xsl:for-each select="@*">
					<li><b><xsl:value-of select="name()"/>: </b> <xsl:value-of select="."/></li>
				</xsl:for-each>
			</ul>
		</div>
	</xsl:template>
	
	<xsl:template match="session" mode="sample">
		<div style="border:1px solid #ccc; margin:5px; padding:5px;">
			<p>Данные сессии:</p>
			<ul>
				<xsl:for-each select="@*">
					<li><b><xsl:value-of select="name()"/>: </b> <xsl:value-of select="."/></li>
				</xsl:for-each>
			</ul>
			<xsl:apply-templates mode="sample"/>
		</div>
	</xsl:template>
	
	<xsl:template match="ErrorLog"/>
	<xsl:template match="errorLogView">
		<table border="1" cellpadding="3" cellspacing="0">
			<tr>
				<th>Время</th>
				<th>Сообщение</th>
				<th>Источник</th>
				<th>Стек вызова</th>
				<th>Местоположение</th>
			</tr>
			<xsl:for-each select="//ErrorLog/error">
				<xsl:sort select="@date" order="descending"/>
				<tr>
					<td><xsl:value-of select="@date"/></td>
					<td><xsl:value-of select="Message"/></td>
					<td><xsl:value-of select="Source"/></td>
					<td><xsl:value-of select="StackTrace"/></td>
					<td><xsl:value-of select="TargetSite"/></td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>
	
	<xsl:template match="newsSample">
		<xsl:variable name="newsDoc" select="document('../cache/news.xml')"/>
		<div class="newsList">
			<h2>NEWS SAMPLE</h2>
			<xsl:for-each select="$newsDoc//newsArticle">
				<div class="article">
					<h3><xsl:value-of select="@date"/>: <xsl:value-of select="@title"/></h3>
					<xsl:apply-templates/>
				</div>
			</xsl:for-each>
		</div>
	</xsl:template>
	
	<xsl:template match="TestXmlEditor">
		<script type="text/javascript"><![CDATA[
			var testNode= {
				_type:"person",
				_attr:{
					fio:"Иванов И.И.",
					age:29
				},
				_ch:[
					{_type:"phone", _attr:{type:"mobile"}, _ch:["444-44-44"]},
					{_type:"phone", _attr:{type:"work"}, _ch:["333-33-33"]},
					{_type:"phone", _ch:["222-33-33"]},
					{_type:"description", _ch:["Работник хороший, и, главное, дисциплинированный."]}
				]
			};
			
			var testTypeDefinition = {
				person:{
					alias:"Сотрудник",
					attributes:{
						fio:{alias:"ФИО", mandatory:true},
						age:{alias:"Возраст"}
					},
					children:{
						phone:{count:[1, 0]},
						description:{count:1}
					}
				},
				phone:{
					alias:"Телефон",
					attributes:{
						type:{alias:"Тип", mandatory:true}
					},
					children:{
						xmlText:{count:1, mandatory:true}
					}
				},
				description:{
					alias:"Описание",
					children:{
						xmlText:{count:1}
					}
				},
				xmlText:{alias:"Текст"}
			};
			
			$(function(){
				$(".xmlEditor").xmlEditor(testNode, testTypeDefinition, function(xml){
					console.log(xml);
				});
			});
		]]>
		</script>
		<div class="xmlEditor"></div>
	</xsl:template>
	
	<xsl:template match="IstraManual">
		<p>Текущая версия сборки: <xsl:value-of select="@version"/></p>
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:template match="AvailableSources">
		<p>Доступные классы источников данных:</p>
		<xsl:call-template name="typesList">
			<xsl:with-param name="list" select="."/>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="AvailableQueries">
		<p>Доступные классы запросов:</p>
		<xsl:call-template name="typesList">
			<xsl:with-param name="list" select="."/>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="AvailableSessionManagers">
		<p>Доступные менеджеры сессий:</p>
		<xsl:call-template name="typesList">
			<xsl:with-param name="list" select="."/>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template name="typesList">
		<xsl:param name="list"/>
		<ol>
			<xsl:for-each select="$list/type">
				<xsl:variable name="tName" select="@name"/>
				<li>
					<xsl:value-of select="$tName"/>
					<xsl:if test="ancestor::*/description[@type=$tName]">
						: <xsl:for-each select="ancestor::*/description[@type=$tName]"><xsl:apply-templates/></xsl:for-each>
					</xsl:if>
				</li>
			</xsl:for-each>
		</ol>
	</xsl:template>
	
</xsl:stylesheet>
