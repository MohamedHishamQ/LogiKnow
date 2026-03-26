import { NextRequest, NextResponse } from 'next/server';

const GEMINI_API_KEY = process.env.GEMINI_API_KEY;
const GEMINI_URL = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${GEMINI_API_KEY}`;

const SYSTEM_PROMPT = `You are MANARA AI Assistant (منارة), a specialized logistics and supply chain knowledge assistant for the MANARA educational platform.

Your expertise covers:
- Logistics terminology (Incoterms, FOB, CIF, CFR, EXW, etc.)
- Maritime trade and shipping
- Supply chain management
- Port operations and management
- Freight forwarding
- Customs and trade compliance
- Warehousing and distribution
- Transportation modes (sea, air, road, rail)
- International trade documentation (Bill of Lading, Letter of Credit, etc.)

Guidelines:
- Be concise, accurate, and educational
- When explaining terms, provide practical real-world examples
- Support Arabic (عربي), English, and French responses based on the user's language
- If asked about topics outside logistics/supply chain, politely redirect to your area of expertise
- Use a friendly, professional tone suitable for students and professionals
- Format responses with clear structure when explaining complex concepts`;

interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

export async function POST(request: NextRequest) {
  try {
    if (!GEMINI_API_KEY) {
      return NextResponse.json(
        { error: 'Gemini API key not configured' },
        { status: 500 }
      );
    }

    const body = await request.json();
    const { messages, locale } = body as { messages: ChatMessage[]; locale: string };

    if (!messages || !Array.isArray(messages) || messages.length === 0) {
      return NextResponse.json(
        { error: 'Messages are required' },
        { status: 400 }
      );
    }

    // Build Gemini-compatible content array
    const contents = [];

    // Add system instruction context
    const languageHint = locale === 'ar' ? 'Respond in Arabic.' : locale === 'fr' ? 'Respond in French.' : 'Respond in English.';

    // Add conversation history
    for (const msg of messages) {
      contents.push({
        role: msg.role === 'assistant' ? 'model' : 'user',
        parts: [{ text: msg.content }],
      });
    }

    const geminiPayload = {
      system_instruction: {
        parts: [{ text: `${SYSTEM_PROMPT}\n\n${languageHint}` }],
      },
      contents,
      generationConfig: {
        temperature: 0.7,
        topP: 0.9,
        topK: 40,
        maxOutputTokens: 1024,
      },
    };

    const geminiResponse = await fetch(GEMINI_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(geminiPayload),
    });

    if (!geminiResponse.ok) {
      const errorData = await geminiResponse.text();
      console.error('Gemini API error:', errorData);
      return NextResponse.json(
        { error: 'Failed to get response from AI' },
        { status: 502 }
      );
    }

    const data = await geminiResponse.json();
    const reply =
      data?.candidates?.[0]?.content?.parts?.[0]?.text || 'I could not generate a response. Please try again.';

    return NextResponse.json({ reply });
  } catch (error) {
    console.error('Chat API error:', error);
    return NextResponse.json(
      { error: 'Internal server error' },
      { status: 500 }
    );
  }
}
