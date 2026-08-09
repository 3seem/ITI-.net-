// =====================================================================
// Retriever — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// The "R" in RAG: Retrieval. Given a question and a knowledge base of
// documents, finds the K documents whose embedding is most similar to
// the question's embedding, using cosine similarity — the standard
// measure for "how close are two vectors in direction," used the same
// way by real vector databases (and by our mock).
// =====================================================================
namespace StudentPortalAI
{
    public class Retriever
    {
        private readonly IEmbeddingProvider _embeddingProvider;
        private readonly List<KnowledgeDocument> _documents;

        public Retriever(IEmbeddingProvider embeddingProvider, List<KnowledgeDocument> documents)
        {
            _embeddingProvider = embeddingProvider;
            _documents = documents;
        }

        public List<RetrievedDocument> TopK(string question, int k)
        {
            // Embed every document AND the question with the SAME
            // provider, so they live in the same vector space and can
            // be compared meaningfully.
            var docVectors = _documents
                .Select(d => (Doc: d, Vector: _embeddingProvider.Embed(d.Text)))
                .ToList();

            var questionVector = _embeddingProvider.Embed(question);

            // If the mock provider's vocabulary grew while embedding the
            // question (a brand-new word appeared), every earlier vector
            // is now shorter than the final vocabulary — resize them all
            // to the same length before comparing (mock-provider-only
            // concern; the real OpenAI provider always returns
            // fixed-length vectors and needs no resizing).
            if (_embeddingProvider is MockEmbeddingProvider mock)
            {
                docVectors = docVectors
                    .Select(dv => (dv.Doc, Vector: mock.Resize(dv.Vector)))
                    .ToList();
                questionVector = mock.Resize(questionVector);
            }

            return docVectors
                .Select(dv => new RetrievedDocument
                {
                    Document = dv.Doc,
                    Score = CosineSimilarity(questionVector, dv.Vector)
                })
                .OrderByDescending(r => r.Score)
                .Take(k)
                .ToList();
        }

        public static double CosineSimilarity(float[] a, float[] b)
        {
            int length = Math.Min(a.Length, b.Length);
            double dot = 0, magnitudeA = 0, magnitudeB = 0;

            for (int i = 0; i < length; i++)
            {
                dot += a[i] * b[i];
                magnitudeA += a[i] * a[i];
                magnitudeB += b[i] * b[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            return dot / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}
