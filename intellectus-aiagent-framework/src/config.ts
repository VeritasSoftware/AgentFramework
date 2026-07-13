import 'reflect-metadata';
import { container } from 'tsyringe';
import OpenAI from 'openai';
import { ConversationalAgentSettings } from './core.ts';

export function addIntellectusAIAgentFramework(settings:(settings: ConversationalAgentSettings) => void, 
                                        registerDependencies?: (myContainer: typeof container) => void): void {
    var mySettings = new ConversationalAgentSettings();
    settings(mySettings);

    if (registerDependencies) {
        registerDependencies(container);
    }

    // Register OpenAI client as singleton
    container.register<OpenAI>(OpenAI, {
        useValue: new OpenAI({  apiKey: mySettings.openAIAPIKey }),
    });

    container.register<string>('OPENAI_MODEL', { useValue: mySettings.openAILLMModel });
}

export { container };