use EnterprisePizza

SELECT	o.id as [OrderId]
		,o.OrderedTimeStamp
		,o.PreppedTimeStamp
		,o.LeftBuildingTimeStamp
		,o.ClientIdentifier
		,o.DeliveredTimeStamp
		,p.Id as [PizzaId]
		,s.Id as [SectionId]
		,g.Id as [IngredientSelectionId]
		,a.Name as [Ingredient]
		,o.IsOrdered
		,o.IsReceivedByStore
FROM	PizzaOrders o
JOIN	Pizzas p on o.Id = p.OrderId
JOIN	Sections s on p.Id = s.PizzaId
JOIN	IngredientSelections g on s.Id = g.SectionId
JOIN	AvailableIngredients a on g.AvailableIngredientId = a.Id

SELECT *
  FROM [dbo].[PizzaOrders]
GO

SELECT *
  FROM [dbo].[Pizzas]
GO

SELECT *
  FROM [dbo].[Sections]
GO

SELECT *
  FROM [dbo].[IngredientSelections]
GO

SELECT *
  FROM [dbo].[AvailableIngredients]
GO

RETURN

DELETE FROM [IngredientSelections]
DELETE FROM [Pizzas]
DELETE FROM [PizzaOrders]
DELETE FROM [Sections]
DELETE FROM [IngredientSelections]
--DELETE FROM [AvailableIngredients]

--update [AvailableIngredients] set IsInStock = 1