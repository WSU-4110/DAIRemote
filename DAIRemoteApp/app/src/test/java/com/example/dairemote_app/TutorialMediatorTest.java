package com.example.dairemote_app;

import static org.junit.jupiter.api.Assertions.*;

import androidx.appcompat.app.AlertDialog;

import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mockito;


class TutorialMediatorTest {

    TutorialMediator tutorialMediator;
    AlertDialog.Builder mockBuilder;

    @BeforeEach
    void setUp() {
        tutorialMediator = new TutorialMediator();
        mockBuilder = Mockito.mock(AlertDialog.Builder.class);
        TutorialMediator.SetBuilder(mockBuilder);
    }

    @AfterEach
    void tearDown() {
        tutorialMediator = null;
    }

    @Test
    void getInstance() {
        TutorialMediator instance1 = TutorialMediator.GetInstance(mockBuilder);
        TutorialMediator instance2 = TutorialMediator.GetInstance(mockBuilder);

        assertNotNull(instance1);
        assertSame(instance1, instance2);
    }

    @Test
    void getBuilder() {
        AlertDialog.Builder result = tutorialMediator.GetBuilder();
        assertNotNull(result);
        assertSame(mockBuilder, result);

    }


    @Test
    void getTutorialOn() {
        tutorialMediator.setTutorialOn(true);
        assertTrue( tutorialMediator.getTutorialOn(), "Tutorial is on");

        tutorialMediator.setTutorialOn(false);
        assertFalse(tutorialMediator.getTutorialOn());

    }

    @Test
    void getCurrentStep() {
        tutorialMediator.setCurrentStep(0);
        assertEquals(0, tutorialMediator.getCurrentStep());

        tutorialMediator.setCurrentStep(1);
        assertEquals(1, tutorialMediator.getCurrentStep());

        tutorialMediator.setCurrentStep(2);
        assertEquals(2, tutorialMediator.getCurrentStep());

        tutorialMediator.setCurrentStep(3);
        assertEquals(3, tutorialMediator.getCurrentStep());

        tutorialMediator.setCurrentStep(4);
        assertEquals(4, tutorialMediator.getCurrentStep());

        tutorialMediator.setCurrentStep(5);
        assertEquals(5, tutorialMediator.getCurrentStep());

        tutorialMediator.setCurrentStep(6);
        assertEquals(6, tutorialMediator.getCurrentStep());

        tutorialMediator.setCurrentStep(7);
        assertEquals(7, tutorialMediator.getCurrentStep());

    }

    @Test
    void setServersPage() {
        ServersPage mockServersPage = Mockito.mock(ServersPage.class);
        tutorialMediator.setServersPage(mockServersPage);
        assertNotNull(mockServersPage);

    }

    @Test
    void checkIfStepCompleted() {
        tutorialMediator.setCurrentStep(0);
        assertTrue(tutorialMediator.checkIfStepCompleted());

        tutorialMediator.setCurrentStep(3);
        assertFalse(tutorialMediator.checkIfStepCompleted());

        tutorialMediator.setCurrentStep(5);
        assertTrue(tutorialMediator.checkIfStepCompleted());

    }


}