using Integra.Space.Language.Analysis.MetadataNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis
{
    class MetadataGenerator
    {
        public EQLQueryMetadata GenerateMetadata(PlanNode executionPlan)
        {
            NodesFinder nf = new NodesFinder();

            // se obtiene el from
            PlanNode fromPlan = nf.FindNode(executionPlan, PlanNodeTypeEnum.ObservableFrom).Single();
            string from = fromPlan.Children[0].Properties["Value"].ToString();

            // se obtienen las columnas del order by
            List<PlanNode> orderBy = nf.FindNode(executionPlan, PlanNodeTypeEnum.EnumerableOrderBy);
            List<PlanNode> orderByDesc = nf.FindNode(executionPlan, PlanNodeTypeEnum.EnumerableOrderByDesc);
            PlanNode[] orderByPlan = new PlanNode[1];

            // solo debe existir un order by, por eso se dejó de esta forma
            if (orderBy.Count == 1)
            {
                orderBy.CopyTo(orderByPlan);
            }

            bool isDescendent = false;
            if (orderByDesc.Count == 1)
            {
                orderByDesc.CopyTo(orderByPlan);
                isDescendent = true;
            }            

            List<PlanNode> tupleProjectionOrderBy = nf.FindNode(executionPlan, PlanNodeTypeEnum.TupleProjection);
            List<string> orderByAlias = new List<string>();
            
            foreach(PlanNode column in tupleProjectionOrderBy)
            {
                orderByAlias.Add(column.Properties["Value"].ToString());
            }



            // se obtienen las columnas de group by
            List<PlanNode> groupBy = nf.FindNode(executionPlan, PlanNodeTypeEnum.EnumerableGroupBy);
            List<PlanNode> tupleProjectionGroupBy = nf.FindNode(executionPlan, PlanNodeTypeEnum.TupleProjection);
            List<string> groupByAlias = new List<string>();

            foreach (PlanNode column in tupleProjectionGroupBy)
            {
                groupByAlias.Add(column.Properties["Value"].ToString());
            }
            
            // se obtienen las columnas del select
            List<PlanNode> selectPlan1 = nf.FindNode(executionPlan, PlanNodeTypeEnum.EnumerableSelectForEnumerable);
            List<PlanNode> selectPlan2 = nf.FindNode(executionPlan, PlanNodeTypeEnum.EnumerableSelectForGroupBy);
            List<PlanNode> selectPlan3 = nf.FindNode(executionPlan, PlanNodeTypeEnum.ObservableSelectForBuffer);
            List<PlanNode> selectPlan4 = nf.FindNode(executionPlan, PlanNodeTypeEnum.ObservableSelectForGroupBy);

            return new EQLQueryMetadata();
        }
    }
}
