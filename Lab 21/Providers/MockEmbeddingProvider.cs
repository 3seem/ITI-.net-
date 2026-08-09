// =====================================================================
// MockEmbeddingProvider — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// A real embedding model (e.g. OpenAI's text-embedding-3-small) is
// trained on billions of words so that "GPA" and "grade point average"
// land near each other in vector space, even though the words are
// completely different. That training is what costs money and needs a
// network call.
//
// This mock does NOT do that — it is a deliberately simplified stand-in
// so the rest of the pipeline (Retriever, RagPipeline) can be built,
// run, and VERIFIED entirely offline, deterministically, for free
// (📌 Students May Ask, Rule 17 disclosure below).
//
// What it does instead: a "bag of words" vector. Every distinct word
// that appears anywhere gets its own position in the vector. A text's
// vector has a 1 in the position of every word it contains, 0
// everywhere else. Two texts that share more of the same WORDS get a
// higher cosine similarity — a crude but real approximation of "these
// are about the same thing," which is enough to demonstrate retrieval
// correctly for our small, controlled classroom vocabulary.
//
// 📌 Students May Ask — "Is this how real embeddings work?"
// No. Real embeddings capture MEANING, not just shared words — they'd
// match "instructor" to "professor" even with zero letters in common.
// This mock only matches on literal shared words. That's the honest
// limitation, named on purpose: it is a stand-in for the SHAPE of the
// job (text in, vector out, similar meaning → similar vector), not a
// toy pretending to be the real algorithm.
// =====================================================================
namespace StudentPortalAI
{
    public class MockEmbeddingProvider : IEmbeddingProvider
    {
        // Shared vocabulary across every call, so the same word always
        // lands in the same vector position, regardless of call order.
        private readonly Dictionary<string, int> _vocabulary = new(StringComparer.OrdinalIgnoreCase);

        public float[] Embed(string text)
        {
            var words = Tokenize(text);

            foreach (var word in words)
            {
                if (!_vocabulary.ContainsKey(word))
                {
                    _vocabulary[word] = _vocabulary.Count;
                }
            }

            var vector = new float[Math.Max(_vocabulary.Count, 1)];
            foreach (var word in words)
            {
                vector[_vocabulary[word]] = 1f;
            }

            return vector;
        }

        // Called by the Retriever after ALL documents are embedded, so
        // every vector gets padded to the final, complete vocabulary
        // size before comparison (cosine similarity needs equal-length
        // vectors).
        public float[] Resize(float[] vector)
        {
            if (vector.Length == _vocabulary.Count)
            {
                return vector;
            }

            var resized = new float[_vocabulary.Count];
            Array.Copy(vector, resized, Math.Min(vector.Length, resized.Length));
            return resized;
        }

        private static string[] Tokenize(string text)
        {
            return text
                .ToLowerInvariant()
                .Split(new[] { ' ', '.', ',', '?', '!', ':', ';', '\'', '"' },
                       StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
